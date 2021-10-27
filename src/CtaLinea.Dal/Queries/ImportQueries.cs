using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class ImportQueries 
        : IImportQueries
    {
        private const string ImportSql_Table = "dbo.Imports AS i";
        private const string ImportDetailSql_Table = "dbo.ImportDetails AS d INNER JOIN dbo.TtServices AS s ON d.ServiceId = s.ServiceId";
        private const string PendingImports_Table = "dbo.vw_PendingImports AS i";

        private CtaDbContext _context;

        public ImportQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        // importazioni
        public async Task<QueryItemList<ImportQueryItem>> GetImportListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<ImportQueryItem>(
                ImportSql_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<ImportQueryItem> GetOneImportAsync(
            Guid id)
        {
            var queryDef = new QueryDefinition<ImportQueryItem>(
                ImportSql_Table,
                null,
                "i.Id = @Id",
                new { Id = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<ImportQueryItem> GetLastPendingImportAsync(
            string importDescr)
        {
            var queryDef = new QueryDefinition<ImportQueryItem>(
                PendingImports_Table,
                null,
                "i.ImportDescr = @ImportDescr",
                new { ImportDescr = importDescr });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }


        // dettagli dell'importazione
        public async Task<QueryItemList<ImportDetailQueryItem>> GetImportDetailListAsync(
            Guid importId,
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<ImportDetailQueryItem>(
                ImportDetailSql_Table,
                filterContext,
                "d.ImportId = @ImportId",
                new { ImportId = importId });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<ImportDetailQueryItem> GetOneImportDetailAsync(
            Guid importId,
            int serviceId)
        {
            var queryDef = new QueryDefinition<ImportDetailQueryItem>(
                ImportDetailSql_Table,
                null,
                "d.ImportID = @ImportId AND d.ServiceId = @ServiceId",
                new { ImportId = importId, ServiceId = serviceId });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
