using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using CtaLinea.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class TtServicesQueries 
        : ITtServicesQueries
    {
        private const string TtServices_Table = "dbo.TtServices s";

        private CtaDbContext _context;

        public TtServicesQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        // Calendari
        public async Task<QueryItemList<TtServiceQueryItem>> GetTtServiceListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<TtServiceQueryItem>(
                TtServices_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<TtServiceQueryItem> GetOneTtServiceAsync(
            int id)
        {
            var queryDef = new QueryDefinition<TtServiceQueryItem>(
                TtServices_Table,
                null,
                "s.Id = @id",
                new { id = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
