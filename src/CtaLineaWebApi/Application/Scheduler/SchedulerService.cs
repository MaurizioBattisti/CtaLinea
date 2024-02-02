using CtaLinea.Model.ScheduledTasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public IEnumerable<ScheduledTaskItem> GetCurrentSchedulerTasks ()
		{
			return _scheduler.GetActualScheduledTasks();
        }

		public async Task<IEnumerable<SchedulerTaskLogItem>> GetTaskLogAsync (
			int id)
		{
			var sql = "SELECT l.* FROM [dbo].[SchedulerTaskLog] l WHERE l.TaskId = @TaskId";
            // salva i dati nel db
            using var conn = _context.GetNewConnection();
            conn.Open();
            return await conn.QueryAsync< SchedulerTaskLogItem>(
                sql,
                new
                {
                    TaskId = id,
                });
        }
		public async Task CleanTaskLogAsync (
			int id,
			DateTime? refDate = null)
		{
            if (refDate == null) refDate = DateTime.Now;

            using var conn = _context.GetNewConnection();
            conn.Open();
            await conn.ExecuteAsync(
                "DELETE [dbo].[SchedulerTaskLog] WHERE TaskId = @TaskId AND Time <= @RefDAte",
                new
                {
                    TaskId = id,
                    RefDate = refDate
                });
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
			newTasks = (from i in newTasks
						select  new ScheduledTaskItem ()
						{
							Active = i.Active,
							ActivityId = i.ActivityId,
							Description = i.Description,

							Id = i.Id,
							Arguments = i.Arguments,
							Frequency = i.Frequency,
							RrequencyMask = i.RrequencyMask,
							Timeout = i.Timeout,

							StartTime = i.StartTime,
							EndTime = i.EndTime,
							Interval = i.Interval,

							LastStart = i.LastStart ?? DateTime.Today.AddYears(-1),
							LastEnd = i.LastEnd ?? DateTime.Today.AddYears(-1)
						});

			await this._scheduler.ReplaceAllTAsks(newTasks);
		}
	}
}
