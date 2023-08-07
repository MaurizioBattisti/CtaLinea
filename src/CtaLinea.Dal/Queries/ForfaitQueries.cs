using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.QueryHelper;
using CtaLinea.Model.QueryModel;
using System.Collections;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class ForfaitQueries 
        : IForfaitQueries
    {
        private const string Sql_ForfaitView = "[dbo].[vw_MultiRunForfaits] f";

        private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;

        public ForfaitQueries(
            CtaDbContext context, 
            IZzRequestConstx zzContext)
        {
            this._context = context;
            this._zzContext = zzContext;
        }

        // Calendari
        public async Task<QueryItemList<ForfaitQueryItem>> GetForfaitListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<ForfaitQueryItem>(
                Sql_ForfaitView,
                filterContext);

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<ForfaitQueryItem> GetOneForfaitAsync(
            int id)
        {
            var queryDef = new QueryDefinition<ForfaitQueryItem>(
                Sql_ForfaitView,
                null,
                "f.ForfaitId = @ForfaitId",
                new { ForfaitId = id });

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
