using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.QueryHelper;
using CtaLinea.Model.QueryModel;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class CollectionPointsQueries 
        : ICollectionPointsQueries
    {
        private const string CollectionPointSql_Table = "[dbo].[vw_CollectionPoints] cp";

        private CtaDbContext _context;

        public CollectionPointsQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        // Collection Point
        public async Task<QueryItemList<CollectionPointQueryItem>> GetCollectionPointListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<CollectionPointQueryItem>(
                CollectionPointSql_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
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

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
