using CtaLinea.Model.QueryModel;
using CtaLinea.QueryModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
	public class RunQueries
		: IRunQueries
	{
		private const string RunItemListSql_Table = "[dbo].[vw_Runs] r";

		private CtaDbContext _context;

		public RunQueries(
			CtaDbContext context)
		{
			this._context = context;
		}

		public async Task<QueryItemList<RunItemQueryModel>> GetRunListAsycn(
			IFilteringContext filterContext)
		{
			var queryDef = new QueryDefinition<RunItemQueryModel>(
				RunItemListSql_Table,
				filterContext);

			IDbConnection conn = this._context.Database.GetDbConnection();
			return await conn.QueryListAsync(
				queryDef)
				.ConfigureAwait(false);
		}
		public async Task<RunItemQueryModel> GetOneRunAsync(
			Guid id)
		{
			var queryDef = new QueryDefinition<RunItemQueryModel>(
				RunItemListSql_Table,
				null,
				"a.RunId = @RunId",
				new { RunId = id });

			IDbConnection conn = this._context.Database.GetDbConnection();
			return await conn.QueryOneAsync(
				queryDef)
				.ConfigureAwait(false);
		}
	}
}
