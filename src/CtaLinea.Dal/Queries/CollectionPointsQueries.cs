using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.QueryHelper;
using CtaLinea.Model.QueryModel;
using System.Collections.Generic;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class CollectionPointsQueries 
        : ICollectionPointsQueries
    {
        private const string CollectionPointSql_Table = "[dbo].[vw_CollectionPoints] cp";

        private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;

        public CollectionPointsQueries(
            CtaDbContext context,
            IZzRequestConstx zzContext)
        {
            this._context = context;
            this._zzContext = zzContext;
        }

        // Collection Point
        public async Task<QueryItemList<CollectionPointQueryItem>> GetCollectionPointListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<CollectionPointQueryItem>(
                CollectionPointSql_Table,
                filterContext);

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<CollectionPointQueryItem> GetOneCollectionPointAsync(
            string  id)
        {
            var queryDef = new QueryDefinition<CollectionPointQueryItem>(
                CollectionPointSql_Table,
                null,
                "cp.CollectionPointId = @CollectionPointId",
                new { CollectionPointId = id });

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
