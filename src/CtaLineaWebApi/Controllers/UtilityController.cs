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

        public UtilityController(
            IMediator mediator,
            IUtilityQueries queries,
            ILogger<UtilityController> logger)
        {
            this._mediator = mediator;
            this._queries = queries;
            this._logger = logger;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("carplanning")]
        public async Task<ActionResult<IEnumerable<CarPlanningItem>>> GetCostsByAssociateAsync(
            [FromQuery] Guid? associateId = null,
            [FromQuery] Guid? carId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null
            )
        {
            var result = await this._queries.GetCarPlanningAsync(
                associateId, carId,
                startDate, endDate
                )
                .ConfigureAwait(false);

            return this.Ok(result);
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("ttservices")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult> UploadTtFileAsync(
            IFormFile file)
        {
            try
            {
                var request = new TtServiceImportRequest(
                    file);
                await this._mediator.Send(request);
            }
            catch (Exception exc)
            {
                var msg = "errore durante il caricamento del file dei servizi TT";
                this._logger.LogError(exc, msg);
                return this.Conflict(msg + "\n" + exc.Message);
            }
            return this.Ok();
        }
    }
}
