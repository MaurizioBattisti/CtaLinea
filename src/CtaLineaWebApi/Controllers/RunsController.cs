using CtaLinea.Model.Runs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;
using CtaLinea.QueryModel;
using CtaLinea.Model.QueryModel;
using CtaLineaWebApi.Application.Commands.Runs;
using MediatR;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Linq.Expressions;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/runs")]
    [Produces(MediaTypeNames.Application.Json)]
    public class RunsController
        : ZControllerBase
    {
        private readonly IRunQueries _queries;
        private readonly ISender _mediator;

        public RunsController(
            ISender mediator,
            IRunQueries queries)
        {
            _mediator = mediator;
			_queries = queries;
		}

        [SwaggerOperation("Elenco delle corse")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RunItemQueryModel>))]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await this._queries.GetRunListAsycn(
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [SwaggerOperation("Dati della singola corsa nel formato della lista")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunItemQueryModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IActionResult> GetDetailedOneAsync(
            Guid id)
        {
            var result = await this._queries.GetOneRunAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [SwaggerOperation("Dati della singola corsa nel formato dell'albero di tutti i dati della corsa")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunItem))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOneAsync(
            Guid id)
        {
            var request = new GetOneRunItemRequest(id);
            var result = await _mediator.Send(request)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [SwaggerOperation("Cre3a una nuova corsa")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> NewOneAsync(
            [FromBody] RunItem model)
        {
            var request = new SaveRunRequest()
            {
                RunId = null,
                RunItem = model
            };

            var result = await this._mediator.Send(request)
                .ConfigureAwait (false);
            if (result.Success == false)
            {
                return BadRequest(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Salvataggio corsa Fallito",
                        Detail = result.Message
                    });
            }

            return this.Created(
                string.Format ("/{0}", result.Data),
                null);
        }
        [SwaggerOperation("Aggiorna i dati di una corsa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateOneAsync(
            Guid id,
            [FromBody] RunItem model)
        {
            var request = new SaveRunRequest()
            {
                RunId = id,
                RunItem = model
            };

            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);
            if (result.Success == false)
            {
                return BadRequest(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Salvataggio corsa Fallito",
                        Detail = result.Message
                    });
            }

            return Ok();
        }
        [SwaggerOperation("Elimina una corsa")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOneAsync(
            Guid id)
        {
            var request = new DeleteRunRequest()
            {
                RunId = id
            };

            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);
            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Salvataggio corsa Fallito",
                        Detail = result.Message
                    });
            }

            return NoContent();
        }

        [SwaggerOperation("Esegue un controllo su tutti i dati di una corsa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        [Route("/check")]
        public async Task<IActionResult> CheckOneAsync(
            [FromBody] RunItem model)
        {
            /*
            var result = await this._queries.GetOneAssociateAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
            **/
            await Task.CompletedTask;
            return this.Created("/il_mio_id", null);
        }

        #region dettagli
        [SwaggerOperation("Elenco  delle varianti di una corsa")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RuntimeVariablesExpression>))]
        [HttpGet]
        [Route("{id}/variations")]
        public async Task<IActionResult> GetRunVariantListAsync(
            Guid id)
        {
            var result = await this._queries.GetRunVariationsAsync(
                id,
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        #endregion
    }
}
