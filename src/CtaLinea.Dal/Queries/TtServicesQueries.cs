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

        private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;

        public TtServicesQueries(
            CtaDbContext context, 
            IZzRequestConstx zzContext)
        {
            this._context = context;
            _zzContext = zzContext; 
        }

        // Calendari
        public async Task<QueryItemList<TtServiceQueryItem>> GetTtServiceListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<TtServiceQueryItem>(
                TtServices_Table,
                filterContext);

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
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

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
