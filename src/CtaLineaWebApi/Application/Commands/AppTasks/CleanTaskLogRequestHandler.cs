using CtaLineaWebApi.Application.Scheduler;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class CleanTaskLogRequestHandler
		: IRequestHandler<CleanTaskLogRequest, bool>
	{
		private readonly CtaDbContext _context;
		private readonly ISchedulerTaskLogger _schedulerLogger;
		private readonly ILogger _logger;

		public CleanTaskLogRequestHandler(
			CtaDbContext context,
			ISchedulerTaskLogger schedulerLogger,
			ILogger<CleanTaskLogRequestHandler> logger
			)
		{
			_schedulerLogger = schedulerLogger;
			_context = context;
			_logger = logger;
		}

		public async Task<bool> Handle(
			CleanTaskLogRequest request, 
			CancellationToken cancellationToken)
		{

			var title = string.Format("Pulizia dati log con le retention gironalier: {0} , settimanale {1}, mensile {2}",
				request.DailyRetention ?? 0,
				request.WeeklyRetention ?? 0,
				request.MonthlyRetention ?? 0
				);

			try
			{
				using var conn = _context.GetNewConnection();
				conn.Open();
				await conn.ExecuteAsync(
					"[dbo].[up_CleanTaskLog]",
					new
					{
					},
					commandType: System.Data.CommandType.StoredProcedure,
					commandTimeout: request.Timeout);

				await _schedulerLogger.LogAsync(title);
			}
			catch (Exception ex)
			{
				await _schedulerLogger.LogAsync(title + " " + ex.Message, "ERROR");
			}
			
			return true;
		}
	}
}
