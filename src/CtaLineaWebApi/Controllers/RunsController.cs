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
using CtaLineaWebApi.Application.Services;
using CtaLinea.Model.Helpers;
using ZzSoft.CtaLinea.Dal.Repositories;
using CtaLineaWebApi.Application.Commands.Tags;
using CtaLinea.Model.Filters;

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
        private readonly IRunRepository _repo;
		private readonly ICompleteRunCheckerService _Checker;

        public RunsController(
            ISender mediator,
            IRunRepository repo,
            ICompleteRunCheckerService checker,
            IRunQueries queries)
        {
            _mediator = mediator;
            _repo = repo;
            _Checker = checker;
			_queries = queries;
		}

		[Authorize(Policy = Constants.Policy_RunView)]
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
		[Authorize(Policy = Constants.Policy_RunView)]
		[SwaggerOperation("Elenco delle corse con filtri avanzati")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RunItemQueryModel>))]
        [HttpPost]
        [Route("advanced")]
        public async Task<IActionResult> GetAllAdvancedAsync(
            [FromBody] RunAdvancedFilters advancedFilters)
        {
            var result = await this._queries.GetRunListAsycn(
                this.FilteringContext,
                advancedFilters)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
		[Authorize(Policy = Constants.Policy_RunView)]
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
		[Authorize(Policy = Constants.Policy_RunView)]
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

		[Authorize(Policy = Constants.Policy_RunEdit)]
		[SwaggerOperation("Cre3a una nuova corsa")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> NewOneAsync(
            [FromBody] RunItem model)
        {
            // per prima cosa contorlli dati in ingresso
            var checkResult = await _Checker.CheckRunAsync(model);
            // controll il sirultato del check
            if (checkResult.Status == CheckStatus.Failed)
            {
                return this.BadRequest(checkResult);
            }

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
		[Authorize(Policy = Constants.Policy_RunEdit)]
		[SwaggerOperation("Aggiorna i dati di una corsa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateOneAsync(
            Guid id,
            [FromBody] RunItem model)
        {
            // per prima cosa contorlli dati in ingresso
            var checkResult = await _Checker.CheckRunAsync(model);
            // controll il sirultato del check
            if (checkResult.Status == CheckStatus.Failed)
            {
                return this.BadRequest(checkResult);
            }

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
		[Authorize(Policy = Constants.Policy_RunEdit)]
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

		[Authorize(Policy = Constants.Policy_RunView)]
		[SwaggerOperation("Esegue un controllo su tutti i dati di una corsa")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RunCheckResult))]
        [HttpPost]
        [Route("check")]
        public async Task<IActionResult> CheckOneAsync(
            [FromBody] RunItem model)
        {
            // per prima cosa contorlli dati in ingresso
            var checkResult = await _Checker.CheckRunAsync(model);
            return this.Ok(checkResult);
        }

		[Authorize(Policy = Constants.Policy_RunEdit)]
		[SwaggerOperation("Decodifica i nodi")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RunNode>))]
        [HttpPost]
        [Route("decodenodes")]
        public async Task<IActionResult> NodeDecodeAsync(
            [FromBody] RunNodeDecodeRequest model)
        {
            var request = new NodeDecodeRequest()
            {
                NodesText = model.NodesText
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            return this.Ok(result);
        }

		#region dettagli
		[Authorize(Policy = Constants.Policy_RunView)]
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

		#region tags
		[Authorize(Policy = Constants.Policy_RunView)]
		[SwaggerOperation("restituisce l'elenco degli id di etichetta associati alla corsa")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<int>))]
		[HttpGet]
		[Route("{id}/tags")]
		public async Task<IActionResult> GetRunTagsAsync(
			Guid id)
		{
            var list = await _repo.GetRunTagsAsync(id);
            return this.Ok(list);
		}
		[Authorize(Policy = Constants.Policy_RunEdit)]
		[SwaggerOperation("aggiorna l'elenco delle etichetta associati alla corsa")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[HttpPost]
		[Route("{id}/tags")]
		public async Task<IActionResult> UpdateTagsAsync(
			Guid id,
            [FromBody] IEnumerable<int> tags)
		{
			var request = new SaveRunTagsRequest()
			{
                RunId = id,
				TagIds = tags
			};
			var result = await this._mediator.Send(request)
				.ConfigureAwait(false);

			if (result.Success == false)
			{
				return Conflict(
					new ValidationProblemDetails(new Dictionary<string, string[]>())
					{
						Title = "Salvataggio etichette Fallito",
						Detail = result.Message
					});
			}

			return Ok();
		}
		#endregion
	}
}
