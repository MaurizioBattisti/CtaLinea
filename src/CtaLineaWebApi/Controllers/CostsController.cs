using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Costs;
using CtaLineaWebApi.Application.Commands.Budgets;
using MediatR;
using ZzSoft.CtaLinea.Dal.Services;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/costs")]
    public class CostsController
        : ZControllerBase
    {
        private readonly ISender _mediator;
        private readonly ICostQueries _queries;
        private readonly ICurrentUserService _currentUSer;

        public CostsController(
            ISender mediator,
            ICurrentUserService currentUser,
            ICostQueries queries)
        {
            this._mediator = mediator;
            this._currentUSer = currentUser;
            this._queries = queries;
        }

        [Authorize(Policy =Constants.Policy_Costs)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("byassociate")]
        public async Task<ActionResult<IEnumerable<CostsByAssociate>>> GetCostsByAssociateAsync(
            [FromQuery] int? contractId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] Guid? associateId = null,
            [FromQuery] Guid? carId = null,
            [FromQuery] Guid? runId = null,
			[FromQuery] string simulationName = null
			)
        {
            var assId = await this._currentUSer.GetUserAssociateId()
                .ConfigureAwait(false);
            if (assId != null)
            {
                associateId = assId;
            }

            var result = await this._queries.GetCostByAssociateAsync(
                contractId,
                startDate, endDate,
                associateId, carId,
                runId,
				simulationName)
                .ConfigureAwait(false);

            return this.Ok(result);
        }

        [Authorize(Policy = Constants.Policy_Costs)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("byrun")]
        public async Task<ActionResult<IEnumerable<CostByRunItem>>> GetCostByRunIdAsync (
            [FromBody] CalcCostsRequest request
            )
        {
            var assId = await this._currentUSer.GetUserAssociateId()
                .ConfigureAwait(false);
            if (assId != null)
            {
                request.ASsociateId = assId;
                request.BudgetName = null;
            }

            var myRequest = new GetCostsByRunRequest()
            {
                RawRwquest = request
            };
            var result = await this._mediator.Send(myRequest)
                .ConfigureAwait(false);

            return this.Ok(result);
        }

        [Authorize(Policy = Constants.Policy_Costs)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("byassociate")]
        public async Task<ActionResult<IEnumerable<CostByAssociateItem>>> GetCostByAssociateASync(
            [FromBody] CalcCostsRequest request
            )
        {
            var assId = await this._currentUSer.GetUserAssociateId()
                .ConfigureAwait(false);
            if (assId != null)
            {
                request.ASsociateId = assId;
                request.BudgetName = null;
            }

            var myRequest = new GetCorstByAssociateRequest()
            {
                BudgetId = null,
                RawRwquest = request
            };
            var result = await this._mediator.Send(myRequest)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
		[Authorize(Policy = Constants.Policy_Costs)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpPost]
		[Route("byassociatebyrun")]
		public async Task<ActionResult<IEnumerable<CostByAssociateByRunItem>>> GetCostByAssociateByRunASync(
			[FromBody] CalcCostsRequest request
			)
		{
			var assId = await this._currentUSer.GetUserAssociateId()
				.ConfigureAwait(false);
			if (assId != null)
			{
				request.ASsociateId = assId;
				request.BudgetName = null;
			}

			var myRequest = new GetCorstByAssociateByRunRequest()
			{
				BudgetId = null,
				RawRwquest = request
			};
			var result = await this._mediator.Send(myRequest)
				.ConfigureAwait(false);

			return this.Ok(result);
		}
	}
}
