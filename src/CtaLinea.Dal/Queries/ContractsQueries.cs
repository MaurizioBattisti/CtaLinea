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

        private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;

        public ContractsQueries(
            CtaDbContext context, 
            IZzRequestConstx zzContext)
        {
            this._context = context;
            _zzContext = zzContext; 
        }

        // Calendari
        public async Task<QueryItemList<ContractQueryItem>> GetContractListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<ContractQueryItem>(
                ContractsSql_Table,
                filterContext);

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
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

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
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

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
