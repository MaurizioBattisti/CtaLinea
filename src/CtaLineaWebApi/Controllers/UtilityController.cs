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
using ZzSoft.CtaLinea.Dal.QueryModel;

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

        public UtilityController(
            IMediator mediator,
            ILogger<UtilityController> logger)
        {
            this._mediator = mediator;
            this._logger = logger;
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
