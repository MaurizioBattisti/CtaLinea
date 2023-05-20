using CtaLinea.Model.Helpers;
using CtaLinea.Model.ModelServices;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.ScheduledTasks;
using CtaLinea.Model.TaskRequest;
using CtaLineaWebApi.Application.Commands.AppTasks;
using CtaLineaWebApi.Application.Scheduler;
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
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Controllers
{
	[Authorize(Policy = Constants.Policy_Tasks)]
	[ApiController]
    [Route("api/scheduledtasks")]
    [Produces(MediaTypeNames.Application.Json)]
    public class ScheduledTasksController
        : ZControllerBase
    {
        private readonly ISender _mediator;
        private readonly ISchedulerService _scheduler;
        private readonly ISchedulerTaskChecker _checker;
        private readonly ISchedulerTaskRepository _repository;
        private readonly ILogger _logger;

        public ScheduledTasksController(
            ISender mediator,
            ISchedulerService scheduler,
            ISchedulerTaskChecker checker,
            ISchedulerTaskRepository repository,
            ILogger<ScheduledTasksController> logger)
        {
            _repository = repository;
            _scheduler = scheduler;
            _checker = checker;
            _mediator = mediator;
            _logger = logger;
        }

        [SwaggerOperation("Restituisce la lista delle attività schedulate")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduledTaskItem>>> GetTaskListAsync()
        {
            await this.ReloadSchedulerASync();
            var list = this._scheduler.GetCurrentSchedulerTasks();

            return this.Ok(list);
        }

        [SwaggerOperation("restituisce i dati di una ttività schedulata")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ScheduledTaskItem>> GetOneTaskAsync(
            int id)
        {
            await this.ReloadSchedulerASync();
            var list = this._scheduler.GetCurrentSchedulerTasks();
            var item = list.Where(x => x.Id == id).SingleOrDefault();
            if (item == null) 
            {
                return this.NotFound();
            }
            return this.Ok(item);
        }

        [SwaggerOperation("Inserisce una nuova attività nello scheduler")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<ActionResult<int>> InsertTaskASync (
            [FromBody] ScheduledTaskItem model)
        {
            // controlla la consistenza dei dati
            var checkResult = await _checker.CheckASync(model)
                .ConfigureAwait (false);
            // controll il sirultato del check
            if (checkResult.Status == CheckStatus.Failed)
            {
                return this.BadRequest(checkResult);
            }

            try
            {
                var id = await this._repository.InsertASync(model)
                    .ConfigureAwait(false);

                // dopo aver aggiornato i dati ricarica lo scheduelr
                await this.ReloadSchedulerASync();

                return this.Ok(id);
            }
            catch (Exception ex)
            {
                return this.Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Inserimetno Attività Fallito" ,
                        Detail = ex.Message
                    });
            }
        }
        [SwaggerOperation("Aggiorna i dati di una attivià dello scheudler")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateTaskAsync(
            [FromRoute] int id,
            [FromBody] ScheduledTaskItem model)
        {
            await this.ReloadSchedulerASync();
            if (this.TaskExists(id) == false)
            {
                return this.NotFound();
            }

            var checkResult = await _checker.CheckASync(model)
                .ConfigureAwait(false);
            // controll il sirultato del check
            if (checkResult.Status == CheckStatus.Failed)
            {
                return this.BadRequest(checkResult);
            }
            
            try
            {
                // aggiorna i dati
                await _repository.UpdateASync(model)
                    .ConfigureAwait (false);

                // dopo aver aggiornato i dati ricarica lo scheduelr
                await this.ReloadSchedulerASync();

                return this.NoContent();
            }
            catch (Exception ex)
            {
                return this.Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Inserimetno Attività Fallito",
                        Detail = ex.Message
                    });
            }
        }
        [SwaggerOperation("Elimina una attivià dallo scheduler")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(
            [FromRoute] int id)
        {
            await this.ReloadSchedulerASync();
            if (this.TaskExists(id) == false)
            {
                return this.NotFound();
            }

            var checkResult = await this.CheckForDeleteAsync(id)
                .ConfigureAwait(false);
            // controll il sirultato del check
            if (checkResult.Status == CheckStatus.Failed)
            {
                return this.BadRequest(checkResult);
            }

            try
            {
                // Elimina l'attività
                await _repository.DeleteASync (id)
                    .ConfigureAwait (false);

                // dopo aver aggiornato i dati ricarica lo scheduelr
                await this.ReloadSchedulerASync();

                return this.NoContent();
            }
            catch (Exception ex)
            {
                return this.Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Inserimetno Attività Fallito",
                        Detail = ex.Message
                    });
            }
        }

        [SwaggerOperation("Restituisce il log dell'attività")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/log")]
        public async Task<ActionResult<IEnumerable<SchedulerTaskLogItem>>> GetTaskLogAsync(
            [FromRoute] int id)
        {
            // dopo aver aggiornato i dati ricarica lo scheduelr
            await this.ReloadSchedulerASync();
            if (this.TaskExists(id) == false)
            {
                return this.NotFound();
            }
            var list = await this._scheduler.GetTaskLogAsync(id)
                .ConfigureAwait(false);
            return this.Ok(list);
        }
        [SwaggerOperation("Elimina il log dell'attività più vecchio di una data")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{id}/log")]
        public async Task<IActionResult> DeleteLogASync(
            [FromRoute] int id,
            [FromQuery] DateTime? refDate = null
            )
        {
            // dopo aver aggiornato i dati ricarica lo scheduelr
            await this.ReloadSchedulerASync();
            if (this.TaskExists(id) == false)
            {
                return this.NotFound();
            }
            await this._scheduler.CleanTaskLogAsync(id, refDate)
                .ConfigureAwait(false);

            return this.NoContent();
        }

        #region funzioni private
        private async Task ReloadSchedulerASync()
        {
            // manda un comando per ricaricare lo scheduler e poi restituisce lo scheduler attualmetne  caricato
            var request = new ReloadSChdulerRequest()
            {
                Timeout = 100
            };
            await this._mediator.Send(request)
                .ConfigureAwait(false);
        }
        private bool TaskExists (int id)
        {
            var list = this._scheduler.GetCurrentSchedulerTasks();
            var item = list.Where(x => x.Id == id).SingleOrDefault();
            return (item != null);
        }
        private async Task<CheckResult> CheckForDeleteAsync(
            int id)
        {
            var errors = new List<CheckResultItem>();
            var warnings = new List<CheckResultItem>();
            var informations = new List<CheckResultItem>();
            var result = new RunCheckResult()
            {
                Status = CheckStatus.Success,
                Errors = errors,
                Warnings = warnings,
                Informations = informations
            };

            if (id == 1)
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "Non è possibile eliminare l'id numero 1",
                    Title = string.Empty,
                });
            }

            // controlla se nella lsita di dettaglaio ci sono erorri
            if (errors.Count > 0) result.Status = CheckStatus.Failed;
            else if (warnings.Count > 0) result.Status = CheckStatus.Warning;
            else if (informations.Count > 0) result.Status = CheckStatus.Information;

            if (errors.Count > 0)
            {
                result.Title = "Errori";
                result.Description = "Errori nella definizione dell'appalto";
            }

            return await Task.FromResult(result);
        }
        #endregion
    }
}
