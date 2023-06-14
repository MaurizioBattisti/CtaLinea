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
        private const string SQL_GetCosts = "[dbo].[up_GetCosts_New]";
        private const string SQL_GetCostsByAssociate = "[dbo].[up_GetCostByAssociate]";
        private const string SQç_GetCostsByRun = "[dbo].[up_GetCostByRun]";

        private readonly CtaDbContext _context;
        private readonly ILogger _logger;

        public BudgetRepository(
            CtaDbContext context,
            ILogger<BudgetRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region gestione della tabella dei budgets
        #endregion

        #region gestione dei costi
        public async Task UpdateBudgetDetaulsAsync(
            CalcCostsRequest request)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

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
                    BudgetType = request.BudgetType
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
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

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
                    BudgetType = request.BudgetType
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = reader.Read<CostByAssociateItem>();
            return await Task.FromResult(items);
        }
        public async Task<IEnumerable<CostByRunItem>> GetRunCostAsync(
            CalcCostsRequest request
            )
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            using var reader = await conn.QueryMultipleAsync(
                SQç_GetCostsByRun,
                param: new
                {
                    RunId = request.RunId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    OutOfPEriod = request.IncludeOutOfPeriod,
                    Suspended = request.IncludeSuspended,
                    RplacedCars = request.UseReplacedCars
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);
            var items = reader.Read<CostByRunItem>();
            return await Task.FromResult(items);
        }
        #endregion
    }
}
