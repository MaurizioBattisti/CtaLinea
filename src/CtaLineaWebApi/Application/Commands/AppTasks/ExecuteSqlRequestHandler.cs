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
	public class ExecuteSqlRequestHandler
        : IRequestHandler<ExecuteSqlRequest, bool>
	{
		private readonly CtaDbContext _context;
		private readonly ISchedulerTaskLogger _schedulerLogger;
		private readonly ILogger _logger;

		public ExecuteSqlRequestHandler(
			CtaDbContext context,
			ISchedulerTaskLogger schedulerLogger,
			ILogger<ExecuteSqlRequestHandler> logger
			)
		{
			_schedulerLogger = schedulerLogger;
			_context = context;
			_logger = logger;
		}

		public async Task<bool> Handle(
            ExecuteSqlRequest request, 
			CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.SqlCommand) == true)
			{
                await _schedulerLogger.LogAsync("Nessun cmando da eseguire");
                return true;
			}
            
			var title = string.Format("Esecuzione SQL: {0} ",
                request.SqlCommand.Substring(0, Math.Min(request.SqlCommand.Length, 200))
                );

            try
            {
                await _schedulerLogger.LogAsync("Inizio " + title);
                using var conn = _context.GetNewConnection();
				conn.Open();
				await conn.ExecuteAsync(
					request.SqlCommand,
					new
					{
					},
					commandType: System.Data.CommandType.Text,
					commandTimeout: request.Timeout);

				await _schedulerLogger.LogAsync("Fine " +  title);
			}
			catch (Exception ex)
			{
				await _schedulerLogger.LogAsync(title + " " + ex.Message, "ERROR");
			}
			
			return true;
		}
	}
}
