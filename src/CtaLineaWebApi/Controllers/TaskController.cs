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
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;

namespace CtaLineaWebApi.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/tasks")]
	[Produces(MediaTypeNames.Application.Json)]
	public class TaskController
		: ZControllerBase
	{
		private readonly ISender _mediator;
		private readonly ISchedulerService _scheduler;
		private readonly ISchedulerTaskLogger _schedulerLogger;
		private readonly ILogger _logger;

		public TaskController(
			ISender mediator,
			ISchedulerService scheduler,
			ISchedulerTaskLogger schedulerLogger,
			ILogger<TaskController> logger)
		{
			_scheduler = scheduler;
			_schedulerLogger = schedulerLogger;
			_mediator = mediator;
			_logger = logger;
		}

		[SwaggerOperation("ricarica lo scheduler")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPost]
		[Route(Constants.Activity_ReloadScheduler + "/{id}" )]
		public async Task<IActionResult> DoSomethingAsync(
			[FromRoute] int id,
			[FromQuery] int? timeout)
		{
			_schedulerLogger.TaskId = id;
			// await _scheduler.StartActivityAsync(Constants.Activity_ReloadScheduler, id);

			var request = new ReloadSChdulerRequest()
			{
				Timeout = timeout ?? 100
			};

			try
			{
				var result = await this._mediator.Send(request)
					.ConfigureAwait(false);
				if (result == false)
				{
					return this.BadRequest("Operazione fallita senza messaggio");
				}
			}
			catch (Exception ex)
			{
				return this.BadRequest(ex);
			}

			// await _scheduler.EndActivityAsycn(Constants.Activity_ReloadScheduler, id);
			return this.NoContent();
		}

		[SwaggerOperation("ricarica lo scheduler")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPost]
		[Route(Constants.Activity_RecalcRunDays + "/{id}")]
		public async Task<IActionResult> RecalcRunDaysAsync(
			[FromRoute] int id,
			[FromQuery] int? timeout,
			[FromBody] RecalcRunDaysTaskRequest taskRequest)
		{
			_schedulerLogger.TaskId = id;
			await _scheduler.StartActivityAsync(Constants.Activity_RecalcRunDays, id);

			var request = new CalcDaysRequest()
			{
				Timeout = timeout ?? 100,
				MaxRuns = taskRequest.MaxRuns ?? 0
			};
			
			try
			{
				var result = await this._mediator.Send(request)
					.ConfigureAwait(false);
				if (result == false)
				{
					return this.BadRequest("Operazione fallita senza messaggio");
				}
			}
			catch (Exception ex)
			{
				return this.BadRequest(ex);
			}

			await _scheduler.EndActivityAsycn(Constants.Activity_RecalcRunDays, id);
			return this.NoContent();
		}

		[SwaggerOperation("Esegue una pulizia del log delle attività")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPost]
		[Route(Constants.Activity_CleanLog + "/{id}")]
		public async Task<IActionResult> CleanTaskLogAsync(
			[FromRoute] int id,
			[FromQuery] int? timeout,
			[FromBody] CleanTaskLogTaskRequest taskRequest)
		{
			_schedulerLogger.TaskId = id;
			await _scheduler.StartActivityAsync(Constants.Activity_CleanLog, id);

			var request = new CleanTaskLogRequest()
			{
				Timeout = timeout ?? 100,
				DailyRetention = taskRequest.DailyRetention,
				WeeklyRetention = taskRequest.WeeklyRetention,
				MonthlyRetention = taskRequest.MonthlyRetention
			};

			try
			{
				var result = await this._mediator.Send(request)
					.ConfigureAwait(false);
				if (result == false)
				{
					return this.BadRequest("Operazione fallita senza messaggio");
				}
			}
			catch (Exception ex)
			{
				return this.BadRequest(ex);
			}

			await _scheduler.EndActivityAsycn(Constants.Activity_CleanLog, id);
			return this.NoContent();
		}

        [SwaggerOperation("Invia mail con le attività della setimana ")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost]
        [Route(Constants.Activity_SendMail + "/{id}")]
        public async Task<IActionResult> SendActivityMAilAsync(
            [FromRoute] int id,
            [FromQuery] int? timeout,
            [FromBody] WeekActivityMailSendRequest taskRequest)
        {
            _schedulerLogger.TaskId = id;
            await _scheduler.StartActivityAsync(Constants.Activity_SendMail, id);

            var request = new SendMAilForWeekActivityRequest()
            {
                Timeout = timeout ?? 100,

                ForseDestination = taskRequest.ForseDestination,
                AssociateId = taskRequest.AssociateId,
                ReferenceDate = taskRequest.ReferenceDate
            };

            try
            {
                var result = await this._mediator.Send(request)
                    .ConfigureAwait(false);
                if (result == false)
                {
                    return this.BadRequest("Operazione fallita senza messaggio");
                }
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex);
            }

            await _scheduler.EndActivityAsycn(Constants.Activity_SendMail, id);
            return this.NoContent();
        }

    }
}
