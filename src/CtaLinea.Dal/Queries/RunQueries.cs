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
        private const string RunVariationList_Table = "[dbo].[vw_RunVariations] v";
        

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
                "r.RunId = @RunId",
				new { RunId = id });

			IDbConnection conn = this._context.Database.GetDbConnection();
			return await conn.QueryOneAsync(
				queryDef)
				.ConfigureAwait(false);
		}

        public async Task<QueryItemList<RunVariationQueryModel>> GetRunVariationsAsync(
            Guid runId,
            IFilteringContext filterContext)
		{
            var queryDef = new QueryDefinition<RunVariationQueryModel>(
                RunVariationList_Table,
                filterContext,
				"v.RunId = @RunId",
				new { RunId = runId });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<RunVariationQueryModel> GetOneVariationAsync(
            Guid id)
		{
            var queryDef = new QueryDefinition<RunVariationQueryModel>(
                RunVariationList_Table,
                null,
                "v.RunVariationId = @RunVariationId",
                new { RunVariationId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);

        }

    }
}
