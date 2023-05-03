using CtaLinea.Model.Base;
using CtaLinea.Model.Contab;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.ModelServices;
using CtaLinea.Model.QueryModel;
using CtaLineaWebApi.Application.Commands.Contracts;
using CtaLineaWebApi.Application.Commands.Forfait;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NPOI.Util;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/forfaits")]
    public class ForfaitController
        : ZControllerBase
    {
        private readonly IForfaitQueries _queries;
        private readonly ISender _mediator;
        private readonly ILogger _logger;

        public ForfaitController(
            ISender mediator,
            IForfaitQueries queries,
            ILogger<ForfaitController> logger)
        {
            this._mediator = mediator;
            this._queries = queries;
            _logger = logger;
        }

        [SwaggerOperation("Elenco dei forfait multi corsa")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForfaitQueryItem>>> GetAllAsync()
        {
            var result = await this._queries.GetForfaitListAsync(
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        
        [SwaggerOperation("dati di un singolo forfati multicorsa")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ForfaitQueryItem>> GetOneAsync(
            int id)
        {
            var result = await this._queries.GetOneForfaitAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }


		[SwaggerOperation("Crea un nuovo forfait multi corsa")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status201Created, 
            Type= typeof(int))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		[Route("{id}")]
		public async Task<IActionResult> AddNewAsync (
            [FromBody] MultiRunForfait model)
        {
            var request = new InsertForfaitRequest()
            {
                Model = model
            };
            var id = this._mediator.Send(request)
                .ConfigureAwait (false);
			string url = string.Format("api/forfaits/{0}", id);

            return this.Created(url, id);
        }

		[SwaggerOperation("modifica i dati di un nuovo forfait multi corsa")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPut]
		[Route("{id}")]
		public async Task<IActionResult> UpdateOneAsync(
            [FromRoute] int id, 
			[FromBody] MultiRunForfait model)
		{
            var request = new UpdateForfaitRequest()
            {
                Id = id,
                Model = model
            };
            await this._mediator.Send (request)
                .ConfigureAwait (false);

			return this.Ok();
		}

		[SwaggerOperation("modifica i dati di un nuovo forfait multi corsa")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> DeleteOneAsync(
			[FromRoute] int id)
		{
            var request = new DeleteForfaitReuqest()
            {
                Id = id
            };
            await this._mediator.Send(request)
                .ConfigureAwait (false);

			return this.NoContent();
		}

		[SwaggerOperation("Imposta il forfait sulle corse")]
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status204NoContent, 
            Type = typeof(CheckResult))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		[Route("setruns")]
		public async Task<IActionResult> AddNewAsync(
	        [FromBody] MultiRunForfaitOperation model)
		{
			if (model.ForfaitId == null
                && (model.RunIds == null
                    || model.RunIds.Count() == 0)
                )
            {
                // ritorna un badrequest
                return this.BadRequest(
                    new CheckResult() {  Description = "non è possibile indicare tutte el corse vuote e anche il forfait vuoto"}
                    ); ;
            }

			var request = new SetDetailRequest()
            {
                ForfaitId = model.ForfaitId,
                RunIds = model.RunIds
            };
            await this._mediator.Send (request)
                .ConfigureAwait (false);   

            return this.NoContent();
		}
	}
}
