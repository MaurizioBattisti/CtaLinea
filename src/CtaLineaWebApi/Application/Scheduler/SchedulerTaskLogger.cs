using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace CtaLineaWebApi.Application.Scheduler
{
	public class SchedulerTaskLogger 
		: ISchedulerTaskLogger,
		IDisposable
	{
		private const string Sql_Log = @"INSERT INTO [dbo].[SchedulerTaskLog]
(TaskId, Tag, Code, Message)
VALUES (@TaskId, @Tag, @Code, @Message)";

		private readonly CtaDbContext _context;
		private readonly ILogger _logger;
		private readonly Guid _tag;
		private readonly IDbConnection _conn;

		public SchedulerTaskLogger(
			CtaDbContext context,
			ILogger<SchedulerTaskLogger> logger)
		{
			_context = context;
			_logger = logger;
			_tag = Guid.NewGuid();
			_conn = _context.GetNewConnection();
			_conn.Open();
		}

		public void Dispose()
		{
			_conn.Dispose();
		}

		public int TaskId { get; set; }

		public async Task LogAsync(
			string message,
			string code = null)
		{
			try
			{
				await _conn.ExecuteAsync(
					Sql_Log,
					new
					{
						TaskId = this.TaskId,
						Tag = _tag,
						Code = code,
						Message = message
					});
			}
			catch (Exception ex)
			{
				this._logger.LogError(ex, "errore durnate il log dell'attività {0}, {1}, {2}", this.TaskId, code ?? string.Empty, message ?? string.Empty);
			}
		}
	}
}
