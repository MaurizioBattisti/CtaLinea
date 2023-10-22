using CtaLineaWebApi.Application.Commands;
using CtaLineaWebApi.Utility.BackGround;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;
using CtaLinea.QueryModel;
using CtaLinea.Model.Costs;
using CtaLinea.Model.Utilities;
using CtaLinea.Model.Checks;
using NPOI.HSSF.Record.Chart;
using ZzSoft.CtaLinea.Dal.Repositories;
using ZzSoft.CtaLinea.Dal.Services;
using CtaLinea.Model.QueryModel;
using Swashbuckle.AspNetCore.Annotations;
using CtaLineaWebApi.Application.Scheduler;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using CtaLinea.Model.Reports;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/utility")]
    public class UtilityController
        : ZControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;

        private readonly IUtilityQueries _queries;
        private readonly ICurrentUserService _currentUSer;
        private readonly IUtilityREpository _utilityRepo;
		private readonly ISchedulerService _schedulerService;
		private readonly ISimulationRepository _simRepo;
        private readonly IReportQueries _reportQueries;

        public UtilityController(
            IMediator mediator,
            IUtilityQueries queries,
			IUtilityREpository utilityRepo,
            ICurrentUserService currentUser,
            ISchedulerService schedulerService,
            ISimulationRepository simRepo,
            IReportQueries reportQueries,
            ILogger<UtilityController> logger)
        {
            this._mediator = mediator;
            this._queries = queries;
            this._utilityRepo = utilityRepo;
            this._currentUSer = currentUser;
			this._schedulerService = schedulerService; ;
			this._simRepo = simRepo;
            this._reportQueries = reportQueries;
            this._logger = logger;
        }

		[Authorize(Policy = Constants.Policy_Planning)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("carplanning")]
        public async Task<ActionResult<IEnumerable<CarPlanningItem>>> GetCarPlanningAsync(
            [FromQuery] Guid? associateId = null,
            [FromQuery] Guid? carId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null
            )
        {
            var assId = await this._currentUSer.GetUserAssociateId()
                .ConfigureAwait(false);
            if (assId != null)
            {
                associateId = assId;
            }

            var result = await this._queries.GetCarPlanningAsync(
                associateId, carId,
                startDate, endDate
                )
                .ConfigureAwait(false);

            return this.Ok(result);
        }
		[Authorize(Policy = Constants.Policy_RunView)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet]
		[Route("runplanning")]
		public async Task<ActionResult<IEnumerable<RunPlanningItem>>> GetRunPlanningAsync(
			[FromQuery] Guid runId,
			[FromQuery] DateTime? startDate = null,
			[FromQuery] DateTime? endDate = null
			)
		{
            var userRun = await this._currentUSer.IsAssociateRun(runId)
                .ConfigureAwait(false);
            if (userRun == false)
            {
                return this.Ok(new List<RunPlanningItem>());
            }

            var result = await this._queries.GetRunPlanningAsync(
				runId,
				startDate, endDate
				)
				.ConfigureAwait(false);

			return this.Ok(result);
		}

		[Authorize(Policy = Constants.Policy_RunView)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("overlappingcars")]
        public async Task<ActionResult<IEnumerable<OverlappingCarItem>>> GetRunOverlappingCarsAsync(
            [FromQuery] Guid runCarId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate
            )
        {
            // TODO: filtrare per ditta se l'utente lo prevede

            var result = await this._queries.GetOverlappingRunCarAsync(
                runCarId,
                startDate, endDate
                )
                .ConfigureAwait(false);

            return this.Ok(result);
        }
		[Authorize(Policy = Constants.Policy_ManageData)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet]
		[Route("globaloverlappingcars")]
		public async Task<ActionResult<IEnumerable<GlobalCarOverlappingItem>>> GetGlobalOverlappingCarsAsync(
			[FromQuery] DateTime? startDate,
			[FromQuery] DateTime? endDate
			)
		{
			// TODO: filtrare per ditta se l'utente lo prevede

			var result = await this._queries.GetGlobalOverlappingRunCarAsync(
				startDate, endDate
				)
				.ConfigureAwait(false);

			return this.Ok(result);
		}

		[Authorize(Policy = Constants.Policy_ViewData)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpGet]
		[Route("carsfordiscontinuation")]
		public async Task<ActionResult<IEnumerable<Guid>>> GetCarsFporDiscontinuationAsync(
			[FromQuery] Guid associateId,
			[FromQuery] DateTime? refDate = null
			)
		{
            var assId = await this._currentUSer.GetUserAssociateId()
                .ConfigureAwait(false);
            if (assId != null)
            {
                associateId = assId.Value;
            }

            var result = await this._utilityRepo.GetCarForDiscontinuationAsync(
				associateId, refDate 
				)
				.ConfigureAwait(false);

			return this.Ok(result);
		}
		[Authorize(Policy = Constants.Policy_ManageData)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPost]
		[Route("replacecars")]
		public async Task<ActionResult> ReplaceCArsASync(
			[FromBody] IDictionary<Guid, Guid > carmap,
			[FromQuery] DateTime? refDate = null
			)
		{
			 await this._utilityRepo.ChangeRunCarDataAsync(
				carmap, refDate
				)
				.ConfigureAwait(false);

            return this.NoContent();
		}

        [Authorize(Policy = Constants.Policy_ManageData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
        [Route("runincongruence")]
        public async Task<ActionResult<IEnumerable<RunIncongruenceModel>>> GetRunIncongruenceAsync(
            [FromBody] RinIncongruenceRequest request
            )
        {
            try
            {
                var model = await this._utilityRepo.GetRunIncongruenceASync(
                   request.RunId,
                   request.StartDate,
                   request.EndDate,
                   request.WhatIncongruence
                   )
                   .ConfigureAwait(false);

                return this.Ok(model);
            }
            catch (Exception ex)
            {
				return BadRequest(
					new ValidationProblemDetails(new Dictionary<string, string[]>())
					{
						Title = "errore caricando i dati delle incongruenze",
						Detail = ex.Message
					});
            }
        }

		[Authorize(Policy = Constants.Policy_ManageData)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(
			StatusCodes.Status200OK, 
			Type=typeof(IEnumerable<string>) 
			)]
		[SwaggerOperation("Elenco delle simulazioni presenti in archivio")]
		[HttpGet]
		[Route("sims")]
		public async Task<ActionResult<IEnumerable<string>>> GetSimulationNamesASync(
			)
		{
			try
			{
				var model = await this._utilityRepo.GetSimulationNamesAsync()
				   .ConfigureAwait(false);

				return this.Ok(model);
			}
			catch (Exception ex)
			{
				return BadRequest(
					new ValidationProblemDetails(new Dictionary<string, string[]>())
					{
						Title = "errore caricando i dati",
						Detail = ex.Message
					});
			}
		}
		[Authorize(Policy = Constants.Policy_ManageData)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(
			StatusCodes.Status204NoContent
			)]		
		[SwaggerOperation("Elimina una simulaizone")]
		[HttpDelete]
		[Route("sims/{simulationName}")]
		public async Task<IActionResult> DeleteSimulationASync(
			[FromRoute] string simulationName
			)
		{
			try
			{
				await this._utilityRepo.DeleteSimulationAsync(
					simulationName)
				   .ConfigureAwait(false);

				return this.NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(
					new ValidationProblemDetails(new Dictionary<string, string[]>())
					{
						Title = "errore caricando i dati",
						Detail = ex.Message
					});
			}
		}

        #region gestione simulaizoni

        [Authorize(Policy = Constants.Policy_ManageData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("simulations/status")]
        public async Task<ActionResult<SimulationStatusResponse>> GetSimulationStatusAsync(
			[FromBody] SimulationRequest request
            )	
        {
			var result = await this._simRepo.GetSimulationStatus (request)
				.ConfigureAwait (false);

            return this.Ok(result);
        }
        [Authorize(Policy = Constants.Policy_ManageData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost]
        [Route("simulations/apply")]
        public async Task<ActionResult<SimulationStatusResponse>> ApplyChangesToSimulationASync(
            [FromBody] ApplyChangesSimulationRequest request
            )
        {
            await this._simRepo.ApplyChangeslSimulationAsync(request)
                .ConfigureAwait(false);

            return this.NoContent();
        }
        [Authorize(Policy = Constants.Policy_ManageData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost]
        [Route("simulations/kmtotal-apply")]
        public async Task<ActionResult<SimulationStatusResponse>> AppluKmTotalChangesToASimulationASync(
            [FromBody] ApplyChangeToSimRequest<KmToMatchCarRequest> request
            )
        {
			if (request.ApplyChanges.Sim_CarMAtchId  == null)
			{
				request.ApplyChanges.Sim_CarMAtchId = Guid.NewGuid();
            }

			// alliena i valori
			request.Filter.SimulationName = request.ApplyChanges.SimulationName;
            request.Filter.ContractId = request.ApplyChanges.ContractId;
            request.Filter.StartDate = request.ApplyChanges.StartDate;
            request.Filter.EndDate = request.ApplyChanges.EndDate;
            request.Filter.Sim_CarMAtchId = request.ApplyChanges.Sim_CarMAtchId;

            await this._simRepo.ApplyKmTotalSimulationAsync(
					request.Filter, 
					request.ApplyChanges)
                .ConfigureAwait(false);

            return this.NoContent();
        }

        [Authorize(Policy = Constants.Policy_ManageData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost]
        [Route("simulations/setdayamounts")]
        public async Task<ActionResult<SimulationStatusResponse>> Simulation_SetDayAmountsASync(
            [FromBody] SimulationSetDayAmountsRequest request
            )
        {
            await this._simRepo.SetDayAmountsAsync(
                    request)
                .ConfigureAwait(false);

            return this.NoContent();
        }
        [Authorize(Policy = Constants.Policy_ManageData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost]
        [Route("simulations/finalize")]
        public async Task<ActionResult<SimulationStatusResponse>> FinalizeSimulationAsync(
            [FromBody] FinalizeSimulationRequest request
            )
        {
            await this._simRepo.FinalizeAsync(
                    request)
                .ConfigureAwait(false);

            return this.NoContent();
        }

        #endregion

        #region reports
        [Authorize(Policy = Constants.Policy_ManageData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("reports/negativekm")]
        public async Task<ActionResult<IEnumerable<NegativeKmItem>?>> GetNegativeKmAsync(
            [FromBody] NegativeKmReportRequest request
            )
        {
            var result = await this._reportQueries.GetNegativeKmAsync(request)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
        #endregion

    }
}
