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
using ZzSoft.CtaLinea.Dal.QueryModel;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/runs")]
    [Produces(MediaTypeNames.Application.Json)]
    public class RunsController
        : ZControllerBase
    {
        public RunsController(
            )
        {
        }

        [SwaggerOperation("Elenco delle corse")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Object>))]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            /*
            var result = await this._queries.GetAssociateListAsync(
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
            */
            
            await Task.CompletedTask;
            return Ok(); 
        }
        [SwaggerOperation("Dati della singola corsa nel formato della lista")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Object))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IActionResult> GetDetailedOneAsync(
            Guid id)
        {
            /*
            var result = await this._queries.GetOneAssociateAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
            **/
            await Task.CompletedTask;
            return Ok();
        }
        [SwaggerOperation("Dati strutturati della singola corsa")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunItem))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOneAsync(
            Guid id)
        {
            /*
            var result = await this._queries.GetOneAssociateAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
            **/
            await Task.CompletedTask;
            return Ok();
        }
        [SwaggerOperation("Cre3a una nuova corsa")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> NewOneAsync(
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
            /*
            var result = await this._queries.GetOneAssociateAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
            **/
            await Task.CompletedTask;
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
            /*
            var result = await this._queries.GetOneAssociateAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
            **/
            await Task.CompletedTask;
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
    }
}
