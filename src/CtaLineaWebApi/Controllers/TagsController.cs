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
using CtaLinea.Model.Calendar;
using ZzSoft.CtaLinea.Dal.Repositories;
using CtaLinea.Model.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using CtaLineaWebApi.Application.Commands.Calendars;
using MediatR;
using CtaLinea.Model.Base;
using CtaLineaWebApi.Application.Commands.Tags;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tags")]
    public class TagsController
        : ZControllerBase
    {
        private readonly ISender _mediator;
        private readonly ITagRepository _repo;
       
        public TagsController(
            ISender mediator,
            ITagRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

		[Authorize(Policy = Constants.Policy_ViewData)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagForRun>>> GetAllTagsAsync()
        {
            var result = await this._repo.GetAllTagsAsync()
                .ConfigureAwait (false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }

		[Authorize(Policy = Constants.Policy_ManageData)]
		[SwaggerOperation("Crea una nuova etichetta per la corsa")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int) )]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> InsertAsync(
            [FromBody] TagForRun model)
        {
            var request = new InsertTagRequest()
            {
                Model = model
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "creazione etichetta Fallito",
                        Detail = result.Message
                    });
            }

            return this.Created(
                string.Format("/{0}", result.Data),
                result.Data);
        }
		[Authorize(Policy = Constants.Policy_ManageData)]
		[SwaggerOperation("Aggiorna i dati di una etichetta")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] TagForRun model)
        {
            model.TagId = id;

            var request = new UpdateTagRequest()
            {
                Model = model
            };
            var result =await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Salvataggio etichetta Fallito",
                        Detail = result.Message
                    });
            }

            return Ok();
        }

		[Authorize(Policy = Constants.Policy_ManageData)]
		[SwaggerOperation("Elimina una etichetta")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteASync(
            [FromRoute] int id)
        {
            var request = new DeleteTagRequest()
            {
                TagId = id
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);
            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Eliminaizone etichetta fallita",
                        Detail = result.Message
                    });
            }

            return NoContent();
        }
    }
}
