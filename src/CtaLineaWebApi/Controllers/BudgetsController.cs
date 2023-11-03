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

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/budgets")]
    public class BudgetsController
        : ZControllerBase
    {
        private readonly ISender _mediator;
        private readonly IBudgetQueries _queries;

        public BudgetsController(
            ISender mediator,
            IBudgetQueries queries)
        {
            this._mediator = mediator;
            this._queries = queries;
        }

        [Authorize(Policy =Constants.Policy_Costs)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BudgetQueryItem>>> GetBudgetListAsync()
        {
			var result = await this._queries.GetBudgetListAsync(
				this.FilteringContext)
				.ConfigureAwait(false);

			return await this.ModelOKAsync(result)
				.ConfigureAwait(false);
		}

		[Authorize(Policy = Constants.Policy_Costs)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CostsByAssociate>> GetOneBudgetItemAsync(
            [FromRoute] int id)
        {
			var result = await this._queries.GetOneBudgetAsync(
				this.FilteringContext,
				id)
				.ConfigureAwait(false);

			return await this.ModelOKAsync(result)
				.ConfigureAwait(false);
		}

		[Authorize(Policy = Constants.Policy_Costs)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> DeleteBudgetAsync(
            [FromRoute] int id)
        {
            var request = new DeleteBudgetRequest()
            {
                Id = id
            };
			await this._mediator.Send(request)
				.ConfigureAwait(false);

			return this.NoContent();
        }
        [Authorize(Policy = Constants.Policy_Costs)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("{id}/detail")]
        public async Task<ActionResult<IEnumerable<CostByAssociateItem>>> GetBudgetDetailAsync(
            [FromRoute] int id,
            [FromBody] CalcCostsRequest request
            )
        {
            var myRequest = new GetCorstByAssociateRequest()
            {
                BudgetId = id,
                RawRwquest = request
            };
            var result = await this._mediator.Send(myRequest)
                .ConfigureAwait(false);

            return this.Ok(result);
        }

        [Authorize(Policy = Constants.Policy_Costs)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("lastcalc/refresh")]
        public async Task<ActionResult> RefreshLastCalcAsync()
        {
            var myRequest = new UpdateBudgetDetailRequest()
            {
                RawRwquest = new CalcCostsRequest
                {
                    RunId = null,
                    StartDate = null, EndDate = null,
                    ASsociateId = null, CarId = null,
                    ContractId = null,
                    BudgetName = "Costi attuali",
                    BudgetType = Constants.BudgetType_LastCalc,
                    ForceFreshData = true
                }
            };
            await this._mediator.Send(myRequest)
                .ConfigureAwait(false);

            return this.NoContent();
        }
    }
}
