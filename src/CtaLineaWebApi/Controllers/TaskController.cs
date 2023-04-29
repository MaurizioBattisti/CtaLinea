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

		[SwaggerOperation("restituisce l'elenco di attività possibili")]
		[ProducesResponseType(StatusCodes.Status200OK, 
			Type = typeof(IEnumerable<string>))]
		[HttpGet]
		public async Task<IActionResult> GetTaskListAsync()
		{
			await Task.CompletedTask;

			var result = new List<string>()
			{
				Constants.Activity_ReloadScheduler
			};

			return this.Ok(result);
		}

		[SwaggerOperation("ricarica lo scheduler")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPost]
		[Route(Constants.Activity_ReloadScheduler + "/{id}" )]
		public async Task<IActionResult> DoSomethingAsync(
			[FromRoute] int id)
		{
			_schedulerLogger.TaskId = id;
			await _scheduler.StartActivityAsync(Constants.Activity_ReloadScheduler, id);

			var request = new ReloadSChdulerRequest()
			{
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

			await _scheduler.EndActivityAsycn(Constants.Activity_ReloadScheduler, id);
			return this.NoContent();
		}

		[SwaggerOperation("ricarica lo scheduler")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[HttpPost]
		[Route(Constants.Activity_RecalcRunDays + "/{id}")]
		public async Task<IActionResult> RecalcRunDaysAsync(
			[FromRoute] int id,
			[FromBody] CalcDaysRequest request)
		{
			_schedulerLogger.TaskId = id;
			await _scheduler.StartActivityAsync(Constants.Activity_RecalcRunDays, id);

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


		
	}
}
