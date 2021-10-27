using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class RunTypesQueries 
        : IRunTypesQueries
    {
        private const string RunTypesSql_Table = "dbo.RunTypes t";

        private CtaDbContext _context;

        public RunTypesQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        // Calendari
        public async Task<QueryItemList<RunTypeQueryItem>> GetRunTypeListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<RunTypeQueryItem>(
                RunTypesSql_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<RunTypeQueryItem> GetOneRunTypeAsync(
            string id)
        {
            var queryDef = new QueryDefinition<RunTypeQueryItem>(
                RunTypesSql_Table,
                null,
                "t.RunTypeId = @RunTypeId",
                new { RunTypeId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
