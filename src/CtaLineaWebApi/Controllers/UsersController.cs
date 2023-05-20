using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.ModelServices;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.ScheduledTasks;
using CtaLinea.Model.TaskRequest;
using CtaLineaWebApi.Application.Commands.AppTasks;
using CtaLineaWebApi.Application.Scheduler;
using CtaLineaWebApi.Auth.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Controllers
{
	[Authorize(Policy = Constants.Policy_Users)]
	[ApiController]
    [Route("api/users")]
    [Produces(MediaTypeNames.Application.Json)]
    public class UsersController
        : ZControllerBase
    {
        private readonly ISender _mediator;
        private readonly IUsersQueries _queries;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public UsersController(
            ISender mediator,
            IUsersQueries queries,
            IUserService userService,
            ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _queries = queries;
            _userService = userService;
            _logger = logger;
        }

        [SwaggerOperation("Elenco degli utenti del sistema")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserQueryModel>>> GetListAsync()
        {
            var result = await this._queries.GetListAsync(
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [SwaggerOperation("restituisce i dati di un singolo utente")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UserQueryModel>> GetOneASync(
            [FromRoute] string id)
        {
            var result = await this._queries.GetOneASync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }

        [SwaggerOperation("Inserisce un nuovo utente")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> InsertOneAsync(
            [FromBody] NewUserModel model)
        {
            // TODO: esegue un contorllo dei dati dell'utente


            try
            {
                await this._userService.InsertNewUSerAsync(model)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Inserimetno utente Fallito",
                        Detail = ex.Message
                    });
            }
            return this.Ok();
        }
        [SwaggerOperation("Aggiorna i dati di un utente")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("{id}")]
        public  async Task<IActionResult> UpdateOneAsync(
            [FromRoute] string id,
            [FromBody] EditUSerModel model)
        {
            // TODO: controlla che l'utente esista



            // TODO: esegue un contorllo dei dati dell'utente



            try
            {
                await this._userService.UpdateUSerAsync(id, model)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "aggiornamento dati utente Fallito",
                        Detail = ex.Message
                    });
            }
            return this.Ok();
        }
        [SwaggerOperation("elimina un utente")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOneAsync(
            [FromRoute] string id)
        {
            // TODO: controlla che l'utente esista




            try
            {
                await this._userService.DeleteUserAsync(id)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "eliminazione utente fallita",
                        Detail = ex.Message
                    });
            }
            return this.NoContent();
        }

        [SwaggerOperation("Aggiorna i dati di un utente")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        [Route("{id}/resetpwd")]
        public async Task<IActionResult> ResetPasswordASync(
            [FromRoute] string id,
            [FromBody] ResetUserPasswordModel model)
        {
            // TODO: controlla che l'utente esista




            try
            {
                await this._userService.SetUserPAsswordAsync(id, model.Password, model.SetMustChange)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Reset password utente fallito",
                        Detail = ex.Message
                    });
            }
            return this.NoContent();
        }
    }
}
