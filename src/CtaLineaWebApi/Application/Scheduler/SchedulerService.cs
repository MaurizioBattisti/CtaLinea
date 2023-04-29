using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Scheduler
{
	public class SchedulerService 
		: ISchedulerService
	{
		private readonly TaskScheduler _scheduler;
		private readonly ILogger _logger;

		public SchedulerService(
			TaskScheduler scheduler,
			ILogger<SchedulerService> logger
			)
		{
			_scheduler = scheduler;
			_logger = logger;
		}

		public async Task StartActivityAsync(string activityId, int id)
		{
			var dt = DateTime.Now;
			// salva i dati nel db

			// li segna anche nello scheduler
			await this._scheduler.SetStartActivityTime(activityId, id, dt);
		}
		public async Task EndActivityAsycn(string activityId, int id)
		{
			var dt = DateTime.Now;
			// salva i dati nel db

			// li segna anche nello scheduler
			await this._scheduler.SetEndActivityTime(activityId, id, dt);
		}
		public async Task ReloadSchedulerAsync()
		{
			var newTasks = new List<ScheduledTaskItem>();

			// carica i dati dal databse

			await this._scheduler.ReplaceAllTAsks(newTasks);
		}
	}
}
