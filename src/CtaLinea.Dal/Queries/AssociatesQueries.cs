using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using CtaLinea.QueryModel;
using ZzSoft.QueryHelper;
using CtaLinea.Model.QueryModel;
using ZzSoft.CtaLinea.Dal.Services;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class AssociatesQueries 
        : IAssociatesQueries
    {
        private const string AssociateSQL_Tables = "dbo.Associates AS a";
        private const string CareSQL_Tables = "dbo.Cars c INNER JOIN dbo.Associates a ON c.ASsociateId = a.AssociateId";
        private const string DriverSQL_Tables = "dbo.Drivers d INNER JOIN dbo.Associates a ON d.ASsociateId = a.AssociateId";

        private readonly  CtaDbContext _context;
        private readonly ICurrentUserService _userSvc;
        private readonly IZzRequestConstx _zzContext;

        public AssociatesQueries(
			ICurrentUserService userSvc,
            IZzRequestConstx zzContext,
            CtaDbContext context)
        {
            this._context = context;
            this._zzContext = zzContext;
            this._userSvc = userSvc;
        }

        // Dite
        public async Task<QueryItemList<AssociateQueryItem>> GetAssociateListAsync(
            IFilteringContext filterContext)
        {
			var idAss = await  _userSvc.GetUserAssociateId ().ConfigureAwait(false);
            string filter = idAss != null ? "a.AssociateId = @AssociateId" : null;
            object args = idAss != null ? new { AssociateId = idAss } : null;

			var queryDef = new QueryDefinition<AssociateQueryItem>(
                AssociateSQL_Tables,
                filterContext,
				filter,
				args);

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<AssociateQueryItem> GetOneAssociateAsync(
            Guid id)
        {
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
            if (idAss != null) id = id == idAss.Value ? idAss.Value :  Guid.Empty;

			var queryDef = new QueryDefinition<AssociateQueryItem>(
                AssociateSQL_Tables,
                null,
                "a.AssociateId = @AssociateId",
                new { AssociateId = id });

			using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }

        // MEzzi
        public async Task<QueryItemList<CarQueryItem>> GetCarListAsync(
            IFilteringContext filterContext)
        {
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			string filter = idAss != null ? "c.AssociateId = @AssociateId" : null;
			object args = idAss != null ? new { AssociateId = idAss } : null;

			var queryDef = new QueryDefinition<CarQueryItem>(
                CareSQL_Tables,
                filterContext, 
                filter,
                args);

			using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<QueryItemList<CarQueryItem>> GetAssociateCarListAsync(
            IFilteringContext filterContext,
            Guid associateId)
        {
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			if (idAss != null) associateId = associateId == idAss.Value ? idAss.Value : Guid.Empty;

			var queryDef = new QueryDefinition<CarQueryItem>(
                CareSQL_Tables,
                filterContext,
				"c.AssociateId = @AssociateId",
                new { AssociateId = associateId });

			using IDbConnection conn = this._context.GetNewConnection();
            conn.Open ();
			return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<CarQueryItem> GetOneCarAsync(
            Guid id)
        {
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			string filter = idAss != null ? "c.AssociateId = @AssociateId AND c.CarId = @CarId" : "c.CarId = @CarId";
			object args = idAss != null ? new { AssociateId = idAss.Value, CarId = id } : new { CarId = id };

			var queryDef = new QueryDefinition<CarQueryItem>(
                CareSQL_Tables,
                null,
                filter,
                args);

			using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }

        // Autisti
        public async Task<QueryItemList<DriverQueryItem>> GetDriverListAsync(
            IFilteringContext filterContext)
        {
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			string filter = idAss != null ? "d.AssociateId = @AssociateId" : null;
			object args = idAss != null ? new { AssociateId = idAss } : null;

			var queryDef = new QueryDefinition<DriverQueryItem>(
                DriverSQL_Tables,
                filterContext,
                filter,
                args);

			using IDbConnection conn = this._context.GetNewConnection();
            conn.Open ();
			return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<QueryItemList<DriverQueryItem>> GetAssociateDriverListAsync(
            IFilteringContext filterContext,
            Guid associateId)
        {
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			if (idAss != null) associateId = associateId == idAss.Value ? idAss.Value : Guid.Empty;
			
            var queryDef = new QueryDefinition<DriverQueryItem>(
                DriverSQL_Tables,
                filterContext,
                "d.AssociateId = @AssociateId",
                new { AssociateId = associateId });

			using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<DriverQueryItem> GetOneDriverAsync(
            Guid id)
        {
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			string filter = idAss != null ? "d.AssociateId = @AssociateId AND d.DriverId = @DriverId" : "d.DriverId = @DriverId";
			object args = idAss != null ? new { AssociateId = idAss.Value, DriverId = id } : new { DriverId = id };

			var queryDef = new QueryDefinition<DriverQueryItem>(
                DriverSQL_Tables,
                null,
                filter,
                args);

			using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
