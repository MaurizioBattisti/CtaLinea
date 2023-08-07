using CtaLinea.Model.Checks;
using CtaLinea.Model.Costs;
using CtaLinea.Model.Utilities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class UtilityQueries 
        : IUtilityQueries
    {
        private const string SQL_up_GetPlanning = "[dbo].[up_GetPlanning]";
        private const string SQL_up_GetRunPlanning = "[dbo].[up_GetRunPlanning]";
        private const string SQç_uo_OverlappingCars = "[dbo].[up_Check_CarOverlappings]";
		private const string SQç_uo_GlobalOverlappingCars = "[dbo].[up_Check_GobalCarOverlappings]";

		private readonly CtaDbContext _context;
        private readonly ILogger _logger;
        private readonly IZzRequestConstx _zzContext;

        public UtilityQueries(
            CtaDbContext context,
            IZzRequestConstx zzContext,
            ILogger<CostQueries> logger
            )
        {
            _context = context;
            _logger = logger;
            _zzContext = zzContext;
        }

        public async Task<IEnumerable<CarPlanningItem>> GetCarPlanningAsync(
            Guid? associateId = null,
            Guid? carId = null,
            DateTime? startDate = null,
            DateTime? endDate = null
            )
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
                SQL_up_GetPlanning,
                param: new
                {
                    AssociateId = associateId,
                    CarId = carId,
                    StartDate = startDate,
                    EndDate = endDate
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = reader.Read<CarPlanningItem>();
            return await Task.FromResult(items);
        }

		public async Task<IEnumerable<RunPlanningItem>> GetRunPlanningAsync(
			Guid runId,
			DateTime? startDate = null,
			DateTime? endDate = null
			)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
				SQL_up_GetRunPlanning,
				param: new
				{
					RunId = runId,
					StartDate = startDate,
					EndDate = endDate
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			var items = reader.Read<RunPlanningItem>();
			return await Task.FromResult(items);
		}

        // carica i dati delle sovrapposizioni dei mezzi
        public async Task<IEnumerable<OverlappingCarItem>> GetOverlappingRunCarAsync (
            Guid runCarId,
            DateTime startDate,
            DateTime endDate)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
                SQç_uo_OverlappingCars,
                param: new
                {
                    RunCarId = runCarId,
                    StartDate = startDate,
                    EndDate = endDate
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = reader.Read<OverlappingCarItem>();
            return await Task.FromResult(items);
        }

		public async Task<IEnumerable<GlobalCarOverlappingItem>> GetGlobalOverlappingRunCarAsync(
			DateTime? startDate,
			DateTime? endDate)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
                SQç_uo_GlobalOverlappingCars,
                param: new
                {
                    StartDate = startDate,
                    EndDate = endDate
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = reader.Read<GlobalCarOverlappingItem>();
            return await Task.FromResult(items);
		}
	}
}
