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
    public class ContractsQueries
        : IContractsQueries
    {
        private const string ContractsSql_Table = "dbo.Contracts c";
        private const string OperationalPEriod_Table = "[dbo].[vw_OperationalPEriods]";

        private CtaDbContext _context;

        public ContractsQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        // Calendari
        public async Task<QueryItemList<ContractQueryItem>> GetContractListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<ContractQueryItem>(
                ContractsSql_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<ContractQueryItem> GetOneContractAsync(
            int id)
        {
            var queryDef = new QueryDefinition<ContractQueryItem>(
                ContractsSql_Table,
                null,
                "c.ContractId = @ContractId",
                new { ContractId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }

        public async Task<QueryItemList<OperationPeriodQueryItem>> GePOperatingPeriodstListAsync (
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<OperationPeriodQueryItem>(
                OperationalPEriod_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
