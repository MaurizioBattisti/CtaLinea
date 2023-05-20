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

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/costs")]
    public class CostsController
        : ZControllerBase
    {
        private readonly ICostQueries _queries;

        public CostsController(
            ICostQueries queries)
        {
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
            [FromQuery] Guid? runId = null
            )
        {
            var result = await this._queries.GetCostByAssociateAsync(
                contractId,
                startDate, endDate,
                associateId, carId,
                runId)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
    }
}
