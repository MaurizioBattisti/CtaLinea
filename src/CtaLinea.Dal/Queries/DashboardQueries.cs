using CtaLinea.Model.Costs;
using CtaLinea.Model.Dashboards;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class DashboardQueries 
        : IDashboardQueries
    {
        private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;
        private readonly ILogger _logger;

        public DashboardQueries (
            CtaDbContext context,
            IZzRequestConstx zzContext,
            ILogger<DashboardQueries> logger
            )
        {
            _context = context;
            _logger = logger;
            _zzContext = zzContext;
        }

        public async Task<CarCountResult?> GetCarCountAsync(
            CarCountRequest request)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
                "[dbo].[up_Dashboard_GetCarCounts]",
                param: new
                {
                    ContractId = request.ContractId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = await reader.ReadSingleOrDefaultAsync<CarCountResult>();
            return await Task.FromResult(items);
        }

        public async Task<IEnumerable<KmDataItem>?> GetKmAsync(
			KmDataItemRequest request)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
				"[dbo].[uo_Dashboard_GetPrevKm]",
				param: new
                {
                    ContractId = request.ContractId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    ConsiderSuspended = request.ConsiderSuspended,
                    RealElastibusKm = request.RealElastibusKm
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = await reader.ReadAsync<KmDataItem>();
            return await Task.FromResult(items);
        }

		public async Task<IEnumerable<CostDataItem>?> GetCostsAsync(
			CostDataItemRequest request)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
			await conn.InitializeSession(this._zzContext);

			using var reader = await conn.QueryMultipleAsync(				
				"[dbo].[uo_Dashboard_GetCosts]",
				param: new
				{
					ContractId = request.ContractId,
					StartDate = request.StartDate,
					EndDate = request.EndDate,
					ConsiderSuspended = request.ConsiderSuspended,
					RealElastibusKm = request.RealElastibusKm,
					SimulationName = request.SimulationName
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			var items = await reader.ReadAsync<CostDataItem>();
			return await Task.FromResult(items);
		}
	}
}
