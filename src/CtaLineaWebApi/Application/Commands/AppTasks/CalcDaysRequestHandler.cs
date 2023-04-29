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
	public class CalcDaysRequestHandler
		: IRequestHandler<CalcDaysRequest, bool>
	{
		private readonly CtaDbContext _context;
		private readonly ISchedulerTaskLogger _schedulerLogger;
		private readonly ILogger _logger;

		public CalcDaysRequestHandler(
			CtaDbContext context,
			ISchedulerTaskLogger schedulerLogger,
			ILogger<CalcDaysRequestHandler> logger
			)
		{
			_schedulerLogger = schedulerLogger;
			_context = context;
			_logger = logger;
		}

		public async Task<bool> Handle(
			CalcDaysRequest request, 
			CancellationToken cancellationToken)
		{
			await _schedulerLogger.LogAsync("pronto a ricalcolare i giorni di " + request.MaxRuns.ToString() + "  corse");

			using var conn = _context.GetNewConnection();
			conn.Open();
			await conn.ExecuteAsync(
				"[dbo].[uo_RecalcRunDays_Massive]",
				new
				{
					ProcessCount = request.MaxRuns,
					RunId = (Guid?) null
				},
				commandType: System.Data.CommandType.StoredProcedure,
				commandTimeout: 1000);

			await _schedulerLogger.LogAsync("Giorni ricalcolati");
			return true;
		}
	}
}
