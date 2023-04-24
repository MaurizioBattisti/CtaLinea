using CtaLinea.Model.Utilities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public class UtilityREpository
		: RepositoryBase
		, IUtilityREpository
	{
		private readonly CtaDbContext _context;
		private readonly ILogger _logger;

		public UtilityREpository(
			CtaDbContext context,
			ILogger<UtilityREpository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<IEnumerable<Guid>> GetCarForDiscontinuationAsync(
			Guid associateId,
			DateTime? refDate = null)
		{
			if (refDate == null) refDate = DateTime.Today;

			using IDbConnection conn = this._context.Database.GetDbConnection();
			conn.Open();

			using var reader = await conn.QueryMultipleAsync(
                "[dbo].[up_GetCarForDiscontinuation]",
				param: new
				{
					AssociateId = associateId,
					RefDate = refDate
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			var items = reader.Read<Guid>();
			return await Task.FromResult(items);
		}

		public async Task ChangeRunCarDataAsync(
			IDictionary<Guid, Guid> replacementMap,
			DateTime? refDate = null
			)
		{
			// trasforma lil dictionary di mappatura in una stringa di mappatura
			string map = string.Join(",", replacementMap.Select(kv => string.Format("ı0}={1}", kv.Key, kv.Value)));

			await Task.CompletedTask;

			// TODO: chiamare la stored proc [dbo].[up_ChangeCarsFromDate]
			using IDbConnection conn = this._context.Database.GetDbConnection();
			conn.Open();

			await conn.ExecuteAsync(
				"[dbo].[up_ChangeCarsFromDate]",
				param: new
				{
					RefDate = refDate,
					CarMapList = map
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			return;
		}
	}
}
