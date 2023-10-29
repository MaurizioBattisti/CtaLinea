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
using CtaLinea.Model.Dashboards;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dahboard")]
    public class DasboardController
        : ZControllerBase
    {
        private readonly ISender _mediator;
        private readonly IDashboardQueries _queries;
        private readonly ICurrentUserService _currentUSer;

        public DasboardController(
            ISender mediator,
            ICurrentUserService currentUser,
            IDashboardQueries queries)
        {
            this._mediator = mediator;
            this._currentUSer = currentUser;
            this._queries = queries;
        }

        [Authorize(Policy =Constants.Policy_ViewData)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("carcount")]
        public async Task<ActionResult<CarCountResult>> GetCarCountAsync(
            [FromBody] CarCountRequest request
			)
        {
            /*
            var assId = await this._currentUSer.GetUserAssociateId()
                .ConfigureAwait(false);
            if (assId != null)
            {
                associateId = assId;
            }
            */

            var result = await this._queries.GetCarCountAsync(
                request)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
        [Authorize(Policy = Constants.Policy_ViewData)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpPost]
        [Route("km")]
        public async Task<ActionResult<IEnumerable<KmDataItem>>> GetKmAsync(
            [FromBody] KmDataItemRequest request
            )
        {
            var result = await this._queries.GetKmAsync(
                request)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
		[Authorize(Policy = Constants.Policy_ViewData)]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[HttpPost]
		[Route("costs")]
		public async Task<ActionResult<IEnumerable<CostDataItem>>> GetCostsAsync(
			[FromBody] CostDataItemRequest request
			)
		{
			var result = await this._queries.GetCostsAsync(
				request)
				.ConfigureAwait(false);

			return this.Ok(result);
		}
	}
}
