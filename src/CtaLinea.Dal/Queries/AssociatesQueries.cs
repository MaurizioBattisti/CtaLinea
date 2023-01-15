using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using CtaLinea.QueryModel;
using ZzSoft.QueryHelper;
using CtaLinea.Model.QueryModel;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class AssociatesQueries 
        : IAssociatesQueries
    {
        private const string AssociateSQL_Tables = "dbo.Associates AS a";
        private const string CareSQL_Tables = "dbo.Cars c INNER JOIN dbo.Associates a ON c.ASsociateId = a.AssociateId";
        private const string DriverSQL_Tables = "dbo.Drivers d INNER JOIN dbo.Associates a ON d.ASsociateId = a.AssociateId";

        private CtaDbContext _context;

        public AssociatesQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        // Dite
        public async Task<QueryItemList<AssociateQueryItem>> GetAssociateListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<AssociateQueryItem>(
                AssociateSQL_Tables,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<AssociateQueryItem> GetOneAssociateAsync(
            Guid id)
        {
            var queryDef = new QueryDefinition<AssociateQueryItem>(
                AssociateSQL_Tables,
                null,
                "a.AssociateId = @AssociateId",
                new { AssociateId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }

        // MEzzi
        public async Task<QueryItemList<CarQueryItem>> GetCarListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<CarQueryItem>(
                CareSQL_Tables,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<QueryItemList<CarQueryItem>> GetAssociateCarListAsync(
            IFilteringContext filterContext,
            Guid associateId)
        {
            var queryDef = new QueryDefinition<CarQueryItem>(
                CareSQL_Tables,
                filterContext,
                "c.AssociateId = @AssociateId",
                new { AssociateId = associateId });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<CarQueryItem> GetOneCarAsync(
            Guid id)
        {
            var queryDef = new QueryDefinition<CarQueryItem>(
                CareSQL_Tables,
                null,
                "c.CarId = @CarId",
                new { CarId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }

        // Autisti
        public async Task<QueryItemList<DriverQueryItem>> GetDriverListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<DriverQueryItem>(
                DriverSQL_Tables,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<QueryItemList<DriverQueryItem>> GetAssociateDriverListAsync(
            IFilteringContext filterContext,
            Guid associateId)
        {
            var queryDef = new QueryDefinition<DriverQueryItem>(
                DriverSQL_Tables,
                filterContext,
                "d.AssociateId = @AssociateId",
                new { AssociateId = associateId });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<DriverQueryItem> GetOneDriverAsync(
            Guid id)
        {
            var queryDef = new QueryDefinition<DriverQueryItem>(
                DriverSQL_Tables,
                null,
                "d.DriverId = @DriverId",
                new { DriverId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
