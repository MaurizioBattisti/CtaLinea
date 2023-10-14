using Azure.Core;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class SimulationRepository
        : RepositoryBase
        , ISimulationRepository
    {
        private readonly CtaDbContext _context;
        private readonly ILogger _logger;
        private readonly IZzRequestConstx _zzContext;

        public SimulationRepository(
            CtaDbContext context,
            IZzRequestConstx zzzContext,
            ILogger<SimulationRepository> logger)
        {
            _context = context;
            _zzContext = zzzContext;
            _logger = logger;
        }

        public async Task<SimulationStatusResponse?> GetSimulationStatus(
            SimulationRequest request)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            var result = await conn.QuerySingleOrDefaultAsync<SimulationStatusResponse>(
                "[dbo].[up_Sim_GetStatus]",
                param: new
                {
                    SimulationName = request.SimulationName,
                    ContractId = request.ContractId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return await Task.FromResult(result);
        }

        public async Task ApplyChangeslSimulationAsync(
            ApplyChangesSimulationRequest applyRe)
        {
            await this.ApplySimulationAsync(
                applyRe,
                null)
                .ConfigureAwait(false);
        }

        public async Task ApplyKmTotalSimulationAsync(
            KmToMatchCarRequest matchRequest,
            ApplyChangesSimulationRequest applyRe)
        {
            await this.ApplySimulationAsync (
                applyRe,
                async (conn, tran, idSim) =>
                {
                    matchRequest.Sim_CarMAtchId = idSim;

                    await this.MatchTotCarAsync (
                        conn, tran, matchRequest)
                        .ConfigureAwait(false);
                })
                .ConfigureAwait (false);
        }

        #region match car funcions
        private async Task MatchTotCarAsync(
            IDbConnection conn, 
            IDbTransaction tran,
            KmToMatchCarRequest request)
        {
            await conn.ExecuteAsync(
                "[dbo].[up_Sim_SelectCarsByTotalKm]",
                param: new
                {
                    SimulationName = request.SimulationName,
                    ContractId = request.ContractId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,

                    Sim_CarMAtchId = request.Sim_CarMAtchId,

                    MinKm = request.MinKm,
                    MaxKm = request.MaxKm,
                    MinKmCContract = request.MinKmCContract,
                    MaxKmContract = request.MaxKmContract,
                    MinKmExtra = request.MinKmExtra,
                    MaxKmExtra = request.MaxKmExtra
                },
                transaction: tran,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);
        }
        #endregion
        #region apply simulation funcion
        private async Task ApplySimulationAsync (
            ApplyChangesSimulationRequest applyRequest,
            Func<IDbConnection, IDbTransaction, Guid, Task> selectCarsFunc = null
            )
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            var tran = conn.BeginTransaction();
            var idSimMatch = Guid.NewGuid();

            try
            {
                // applica la funzione per selezioanre i mezzi
                if (selectCarsFunc != null)
                {
                    applyRequest.Sim_CarMAtchId = idSimMatch;

                    await selectCarsFunc(conn, tran, idSimMatch)
                        .ConfigureAwait (false);
                }

                await conn.ExecuteAsync(
                    "[dbo].[up_Sim_SetCosts]",
                    param: new
                    {
                        SimulationName = applyRequest.SimulationName,
                        ContractId = applyRequest.ContractId,
                        StartDate = applyRequest.StartDate,
                        EndDate = applyRequest.EndDate,

                        MinCapacity = applyRequest.MinCapacity,
                        MaxCapacity = applyRequest.MaxCapacity,

                        Sim_CarMAtchId = applyRequest.Sim_CarMAtchId,

                        BasePriceLimit = applyRequest.BasePriceLimit,
                        BasePriceNewVAl = applyRequest.BasePriceNewVAl,
                        BasePriceAddVal = applyRequest.BasePriceAddVal,
                        BasePriceFacto = applyRequest.BasePriceFacto,

                        ExtraPriceLimit = applyRequest.ExtraPriceLimit,
                        ExtraPriceNewVAl = applyRequest.ExtraPriceNewVAl,
                        ExtraPriceAddVal = applyRequest.ExtraPriceAddVal,
                        ExtraPriceFacto = applyRequest.ExtraPriceFacto,

                        DefKmPrice = applyRequest.DefKmPrice,
                        DefKmExtraPrice = applyRequest.DefKmExtraPrice,

                        Overwrite = applyRequest.Overwrite
                    },
                    transaction: tran,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 600);

                tran.Commit();
                tran = null;
            }
            finally
            {
                tran?.Rollback();
            }
        }
        #endregion
    }
}
