using Azure.Core;
using CtaLinea.Model.Costs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class BudgetRepository
        : RepositoryBase
        , IBudgetRepository
    {
        private const string Sql_BudgetTable = "[dbo].[Budgets]";

        private const string SQL_GetCosts = "[dbo].[up_GetCosts_New]";
        private const string SQL_GetCostsByAssociate = "[dbo].[up_GetCostByAssociate]";
		private const string SQL_GetCostsByAssociateByRun = "[dbo].[up_GetCostByAssociateByRun]";
		private const string SQç_GetCostsByRun = "[dbo].[up_GetCostByRun]";

        private readonly CtaDbContext _context;
        private readonly ILogger _logger;
        private readonly IZzRequestConstx _zzContext;

        public BudgetRepository(
            CtaDbContext context,
            IZzRequestConstx zzzContext,
            ILogger<BudgetRepository> logger)
        {
            _context = context;
            _logger = logger;
            _zzContext = zzzContext;
        }

        #region gestione della tabella dei budgets
        public async Task<bool> DeleteBudgetAsync (
            int id)
        {
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
			await conn.InitializeSession(this._zzContext);

			await conn.ExecuteAsync(
				"DELETE FROM " + Sql_BudgetTable + " WHERE BudgetId = @BudgetId",
				new
				{
					BudgetId = id
				});

			return true;
		}

		#endregion
		#region gestione dei costi
		public async Task UpdateBudgetDetaulsAsync(
            CalcCostsRequest request)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            this._zzContext.Override(request.ContractId, request.StartDate, request.EndDate);
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
                SQL_GetCosts,
                param: new
                {
                    ContractId = request.ContractId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    AssociateId = request.ASsociateId,
                    CarId = request.CarId,
                    OutOfPEriod = request.IncludeOutOfPeriod,
                    Suspended = request.IncludeSuspended,
                    RplacedCars = request.UseReplacedCars,
                    BudgetName = request.BudgetName,
                    BudgetType = request.BudgetType,
                    SimulationName = request.SimulationName
				},
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return;
        }
		public async Task<IEnumerable<CostByAssociateItem>> GEtCostsByAssociateAsync(
			int? budgetId,
			CalcCostsRequest request
			)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            this._zzContext.Override(request.ContractId, request.StartDate, request.EndDate);
            await conn.InitializeSession(this._zzContext);

			using var reader = await conn.QueryMultipleAsync(
				SQL_GetCostsByAssociate,
				param: new
				{
					BudgetId = budgetId,
					ContractId = request.ContractId,
					StartDate = request.StartDate,
					EndDate = request.EndDate,
					AssociateId = request.ASsociateId,
					CarId = request.CarId,
					OutOfPEriod = request.IncludeOutOfPeriod,
					Suspended = request.IncludeSuspended,
					RplacedCars = request.UseReplacedCars,
					BudgetName = request.BudgetName,
					BudgetType = request.BudgetType,
					SimulationName = request.SimulationName
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			var items = reader.Read<CostByAssociateItem>();
			return await Task.FromResult(items);
		}
		public async Task<IEnumerable<CostByAssociateByRunItem>> GEtCostsByAssociateByRunAsync(
			int? budgetId,
			CalcCostsRequest request
			)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            this._zzContext.Override(request.ContractId, request.StartDate, request.EndDate);
            await conn.InitializeSession(this._zzContext);

			using var reader = await conn.QueryMultipleAsync(
				SQL_GetCostsByAssociateByRun,
				param: new
				{
					BudgetId = budgetId,
					ContractId = request.ContractId,
					StartDate = request.StartDate,
					EndDate = request.EndDate,
					AssociateId = request.ASsociateId,
					CarId = request.CarId,
					OutOfPEriod = request.IncludeOutOfPeriod,
					Suspended = request.IncludeSuspended,
					RplacedCars = request.UseReplacedCars,
					BudgetName = request.BudgetName,
					BudgetType = request.BudgetType,
					SimulationName = request.SimulationName
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			var items = reader.Read<CostByAssociateByRunItem>();
			return await Task.FromResult(items);
		}
		
		public async Task<IEnumerable<CostByRunItem>> GetRunCostAsync(
            CalcCostsRequest request
            )
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            this._zzContext.Override(request.ContractId, request.StartDate, request.EndDate);
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
                SQç_GetCostsByRun,
                param: new
                {
                    RunId = request.RunId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    OutOfPEriod = request.IncludeOutOfPeriod,
                    Suspended = request.IncludeSuspended,
                    RplacedCars = request.UseReplacedCars,
					SimulationName = request.SimulationName
				},
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);
            var items = reader.Read<CostByRunItem>();
            return await Task.FromResult(items);
        }
        #endregion
    }
}
