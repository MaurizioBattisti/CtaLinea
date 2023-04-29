using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace CtaLineaWebApi.Application.Scheduler
{
	public class SchedulerService 
		: ISchedulerService
	{
		private readonly TaskScheduler _scheduler;
		private readonly CtaDbContext _context;
		private readonly ILogger _logger;

		public SchedulerService(
			TaskScheduler scheduler,
			CtaDbContext context,
			ILogger<SchedulerService> logger
			)
		{
			_context = context;
			_scheduler = scheduler;
			_logger = logger;
		}

		public async Task StartActivityAsync(string activityId, int id)
		{
			var dt = DateTime.Now;
			// salva i dati nel db
			using var conn = _context.GetNewConnection();
			conn.Open();
			await conn.ExecuteAsync(
				"UPDATE [dbo].[SchedulerTasks] SET LastStart = @LastStart WHERE Id = @Id",
				new
				{
					LastStart = dt,
					Id = id,
				});

			// li segna anche nello scheduler
			await this._scheduler.SetStartActivityTime(activityId, id, dt);
		}
		public async Task EndActivityAsycn(string activityId, int id)
		{
			var dt = DateTime.Now;
			// salva i dati nel db
			using var conn = _context.GetNewConnection();
			conn.Open();
			await conn.ExecuteAsync(
				"UPDATE [dbo].[SchedulerTasks] SET LastEnd = @LastEnd WHERE Id = @Id",
				new
				{
					LastEnd = dt,
					Id = id,
				});

			// li segna anche nello scheduler
			await this._scheduler.SetEndActivityTime(activityId, id, dt);
		}
		public async Task ReloadSchedulerAsync()
		{
			// carica i dati dal databse
			using var conn = _context.GetNewConnection ();
			conn.Open ();
			var newTasks = await conn.QueryAsync<ScheduledTaskItem>(
				"SELECT * FROM [dbo].[SchedulerTasks]",
				null);

			await this._scheduler.ReplaceAllTAsks(newTasks);
		}
	}
}
