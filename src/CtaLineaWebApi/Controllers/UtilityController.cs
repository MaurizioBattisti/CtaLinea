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

        public UtilityController(
            IMediator mediator,
            IUtilityQueries queries,
			IUtilityREpository utilityRepo,
            ICurrentUserService currentUser,
            ILogger<UtilityController> logger)
        {
            this._mediator = mediator;
            this._queries = queries;
            this._utilityRepo = utilityRepo;
            this._currentUSer = currentUser;
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
        public async Task<ActionResult<IEnumerable<OverlappingCarItem>>> GetRunPlanningAsync(
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
    }
}
