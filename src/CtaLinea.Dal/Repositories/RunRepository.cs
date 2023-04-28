using CtaLinea.Model.Base;
using CtaLinea.Model.External;
using CtaLinea.Model.Runs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.Model.Runs;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class RunRepository 
        : RepositoryBase
        , IRunRepository
    {
        private const string SQL_up_GetOneRunItem = "[dbo].[up_GetOneRunItem]";
        
        private const string SQL_Table_Runs = "[dbo].[Runs]";
        private const string SQL_Table_RunVariations = "[dbo].[RunVariations]";
        private const string SQL_Table_RunCalendars = "[dbo].[RunVariationCalendars]";
        private const string SQL_Table_RunNodes = "[dbo].[RunNodes]";
        private const string SQL_Table_RunPeriods = "[dbo].[RunPeriods]";
        private const string SQL_Table_RunAdditionalDays = "[dbo].[RunAdditionalDays]";
        private const string SQL_Table_RunCars = "[dbo].[RunCars]";
        private const string SQL_Table_RunCarCosts = "[dbo].[RunCarCosts]";
        private const string SQL_Table_CarReplacements = "[dbo].[RunCarReplacements]";
        private const string SQL_Table_CarReplacementDetails = "[dbo].[RunCarReplacementDetails]";
        private const string SQL_Table_RunSuspensions = "[dbo].[RunSuspensions]";

        private const string SQL_Table_RunTags = "[dbo].[RunTags]";

		private readonly CtaDbContext _context;
        private readonly ILogger _logger;

        public RunRepository(
            CtaDbContext context,
            ILogger<RunRepository> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RunItem> GetOneRunItemAsync(
            Guid runId)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();

            return await this.InternalGetOneRunItemAsync(
                runId,
                conn, null
                );
        }

        public async Task DeleteRunAsync (
            Guid runId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            var tran = conn.BeginTransaction();

            try
            {
                // elimina tutta una corsa
                await this.DeleteTableAsync(
                    SQL_Table_Runs,
                    conn, tran,
                    this.GetRunKey(runId)
                    );

                tran.Commit();
                tran = null;
            }
            finally
            {
                if (tran != null ) 
                { 
                    tran.Rollback();
                }
            }
        }
        public async Task SaveRuAsync(
            RunItem runItem)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            var tran = conn.BeginTransaction();

            // se l'id della corsa è vuoto n ne crea uno nuovo
            if (runItem.RunId == Guid.Empty) runItem.RunId = Guid.NewGuid();

            try
            {
                var oldRun = await this.InternalGetOneRunItemAsync(runItem.RunId, conn, tran, false);
                if (oldRun == null)
                {
                    // è un nuovo inserimetno
                    #region inserisce la corsa
                    await this.InsertTableAsync(
                        SQL_Table_Runs,
                        conn, tran,
                        this.JoinObjects(
                            this.GetRunKey(runItem.RunId),
                            this.GetRunData(runItem)
                            )
                        );
                    #region inserisce tutte le varianti
                    if (runItem.Variations != null)
                    {
                        foreach (var v in runItem.Variations)
                        {
                            await this.InsertVariationAsync(runItem.RunId, v, conn, tran);
                        }
                    }
                    #endregion

                    #region inserisce i periodi di operatività dei mezzi
                    if (runItem.SubPeriods != null)
                    {
                        foreach (var period in runItem.SubPeriods)
                        {
                            await this.InsertPeriodAsync(runItem.RunId, period, conn, tran);
                        }
                    }
                    #endregion

                    #region inserisce le sospensioni
                    if (runItem.Suspensions != null)
                    {
                        foreach (var suspension in runItem.Suspensions)
                        {
                            await this.InsertSuspensionAsync(runItem.RunId, suspension, conn, tran);
                        }
                    }
                    #endregion

                    #region inserisce i giorni addizionali
                    if (runItem.AdditionalDays != null)
                    {
                        foreach (var addDay in runItem.AdditionalDays)
                        {
                            await this.InsertDayAsync(runItem.RunId, addDay, conn, tran);
                        }
                    }
                    #endregion

                    #endregion
                }
                else
                {
                    // è un update
                    #region aggiorna i dati della corsa
                    await this.UpdateTableAsync(
                        SQL_Table_Runs,
                        conn, tran,
                        this.GetRunKey(runItem.RunId),
                        this.GetRunData(runItem)
                        );
                    #endregion

                    #region gestisce le variazioni
                    // inserisce le nuove variatni
                    await this.FindNewAsync(
                        oldRun.Variations,
                        runItem.Variations,
                        (v) => v.RunVariationId,
                        async (variant) => await this.InsertVariationAsync(runItem.RunId, variant, conn, tran)
                        );

                    // aggiorna le varianti
                    await this.FindUpdatedAsync(
                        oldRun.Variations,
                        runItem.Variations,
                        (v) => v.RunVariationId,
                        (n, o) => o.RunVariationId == n.RunVariationId,
                        async (variant, oldVariant) =>
                        {
                            await this.UpdateVariationAsync(runItem.RunId, variant, conn, tran);

                            // inserisce i nuovi calendari
                            await this.FindNewAsync(
                                oldVariant.Calendars,
                                variant.Calendars,
                                (v) => v,
                                async (cal) => await this.InsertCalendarAsync(variant.RunVariationId, cal, conn, tran)
                                );
                            // elimina i calendari non più usati
                            await this.FindDeletedAsync(
                                oldVariant.Calendars,
                                variant.Calendars,
                                (v) => v,
                                async (k) => await this.DeleteCalendarAsync(variant.RunVariationId, k, conn, tran)
                                );

                            // inserisce i nuovi nodi
                            await this.FindNewAsync(
                                oldVariant.Nodes,
                                variant .Nodes,
                                (v) => v.RunNodeId,
                                async (node) => await this.InsertNodeAsync(variant.RunVariationId, node, conn, tran)
                                );

                            // aggiorna i nodi
                            await this.FindUpdatedAsync(
                                oldVariant.Nodes,
                                variant.Nodes,
                                (v) => v.RunNodeId,
                                (n, o) => o.RunNodeId == n.RunNodeId,
                                async (node, oldNode) => await this.UpdateNodeAsync (variant.RunVariationId, node, conn, tran)
                                );

                            // elimina i nodi da cancellare
                            await this.FindDeletedAsync(
                                oldVariant.Nodes,
                                variant.Nodes,
                                (v) => v.RunNodeId,
                                async (k) => await this.DeleteVariationAsync(variant.RunVariationId, k, conn, tran)
                                );
                        });

                    // identifica le varianti da eliminare
                    await this.FindDeletedAsync(
                        oldRun.Variations,
                        runItem.Variations,
                        (v) => v.RunVariationId,
                        async (k) => await this.DeleteVariationAsync(runItem.RunId, k, conn, tran)
                        );
                    #endregion

                    #region gestisce i periodi
                    // inserisce i nuovi periodi
                    await this.FindNewAsync(
                        oldRun.SubPeriods,
                        runItem.SubPeriods,
                        (v) => v.RunPeriodId,
                        async (period) => await this.InsertPeriodAsync(runItem.RunId, period, conn, tran)
                        );

                    // aggiorna i periodi
                    await this.FindUpdatedAsync(
                        oldRun.SubPeriods,
                        runItem.SubPeriods,
                        (v) => v.RunPeriodId,
                        (n, o) => o.RunPeriodId == n.RunPeriodId,
                        async (period, oldperiod) =>
                        {
                            await this.UpdatePeriodAsync(runItem.RunId, period, conn, tran);

                            // inserisce i nuovi mezzi
                            await this.FindNewAsync(
                                oldperiod.Cars,
                                period.Cars,
                                (v) => v.RunCarId,
                                async (car) => await this.InsertRunCarAsync(period.RunPeriodId, car, conn, tran)
                                );

                            // aggiorna i mezzi da aggiornare
                            await this.FindUpdatedAsync(
                                oldperiod.Cars,
                                period.Cars,
                                (v) => v.RunCarId,
                                (n, o) => o.RunCarId == n.RunCarId,
                                async (car, oldCar) =>
                                {
                                    await this.UpdateRunCarAsync(period.RunPeriodId, car, conn, tran);

                                    // inserisce i nuovi costi
                                    await this.FindNewAsync(
                                        oldCar.CarCosts,
                                        car.CarCosts,
                                        (v) => v.RunCarCostId,
                                        async (cost) => await this.InsertCarCostAsync(car.RunCarId, cost, conn, tran)
                                        );

                                    // modifica i costi
                                    await this.FindUpdatedAsync(
                                        oldCar.CarCosts,
                                        car.CarCosts,
                                        (v) => v.RunCarCostId,
                                        (n, o) => o.RunCarCostId == n.RunCarCostId,
                                        async (cost, oldCost) => await this.UpdateCarCostAsync(car.RunCarId, cost, conn, tran)
                                        );

                                    // elimina i costi da eliminare
                                    await this.FindDeletedAsync(
                                        oldCar.CarCosts,
                                        car.CarCosts,
                                        (v) => v.RunCarCostId,
                                        async (k) => await this.DeleteCarCostAsync(period.RunPeriodId, k, conn, tran)
                                        );
                                });

                            // inserisce le nuove sostituzioni
                            await this.FindNewAsync(
                                oldperiod.CarReplacements,
                                period.CarReplacements,
                                (v) => v.CarReplacementId,
                                async (replacement) => await this.InsertReplacementAsync(period.RunPeriodId, replacement, conn, tran)
                                );

                            // aggiorna le sostituzioni
                            await this.FindUpdatedAsync(
                                oldperiod.CarReplacements,
                                period.CarReplacements,
                                (v) => v.CarReplacementId,
                                (n, o) => o.CarReplacementId == n.CarReplacementId,
                                async (replacement, oldreplacement) =>
                                {
                                    await this.UpdateReplacementAsync(period.RunPeriodId, replacement, conn, tran);

                                    // aggiorna il dettaglio delle sostituzioni
                                    await this.UpdateReplacementDetailsAsync(replacement, conn, tran);
                                });

                            // elimina le sostituzioni
                            await this.FindDeletedAsync(
                                oldperiod.CarReplacements,
                                period.CarReplacements,
                                (v) => v.CarReplacementId,
                                async (k) => await this.DeleteReplacementAsync(period.RunPeriodId, k, conn, tran)
                                );

                            // elimina i mezzi
                            await this.FindDeletedAsync(
                                oldperiod.Cars,
                                period.Cars,
                                (v) => v.RunCarId,
                                async (k) => await this.DeleteRunCarAsync(period.RunPeriodId, k, conn, tran)
                                );
                        });

                    // elimina i periodi da cancellare
                    await this.FindDeletedAsync(
                        oldRun.SubPeriods,
                        runItem.SubPeriods,
                        (v) => v.RunPeriodId,
                        async (k) => await this.DeletePeriodAsync(runItem.RunId, k, conn, tran)
                        );
                    #endregion

                    #region gestisce le sospensioni
                    // inserisce le nuove sospensioni
                    await this.FindNewAsync(
                        oldRun.Suspensions,
                        runItem.Suspensions,
                        (v) => v.RunSuspensionId,
                        async (suspension) => await this.InsertSuspensionAsync(runItem.RunId, suspension, conn, tran)
                        );

                    // aggiorna le sospensioni
                    await this.FindUpdatedAsync(
                        oldRun.Suspensions,
                        runItem.Suspensions,
                        (v) => v.RunSuspensionId,
                        (n, o) => o.RunSuspensionId == n.RunSuspensionId,
                        async (suspension, oldsuspension) =>
                        {
                            await this.UpdateSuspensionAsync(runItem.RunId, suspension, conn, tran);
                        });

                    // identifica le sospensioni da eliminare
                    await this.FindDeletedAsync(
                        oldRun.Suspensions,
                        runItem.Suspensions,
                        (v) => v.RunSuspensionId,
                        async (k) => await this.DeleteSuspensionAsync(runItem.RunId, k, conn, tran)
                        );
                    #endregion

                    #region gestisce i giorni addizionali
                    // inserisce i nuovi giorni addizionali
                    await this.FindNewAsync(
                        oldRun.AdditionalDays,
                        runItem.AdditionalDays,
                        (v) => v.Day,
                        async (addDay) => await this.InsertDayAsync(runItem.RunId, addDay, conn, tran)
                        );

                    // aggionra i giorni addizionali
                    await this.FindUpdatedAsync(
                        oldRun.AdditionalDays,
                        runItem.AdditionalDays,
                        (v) => v.Day,
                        (n, o) => o.Day == n.Day,
                        async (addDay, oldAddDay) =>
                        {
                            await this.UpdateDayAsync(runItem.RunId, addDay, conn, tran);
                        });

                    // identifica le giorante addizionali da eliminare
                    await this.FindDeletedAsync(
                        oldRun.AdditionalDays,
                        runItem.AdditionalDays,
                        (v) => v.Day,
                        async (k) => await this.DeleteDayAsync(runItem.RunId, k, conn, tran)
                        );
                    #endregion
                }

                // indica che il ricalcolo dei gironi è necessario
                await this.AddToRecalcNeededAsync(
                    conn, tran,
                    runItem.RunId);

                tran.Commit();
                tran = null;
            }
            finally
            {
                if (tran != null ) 
                { 
                    tran.Rollback ();
                }
            }
        }

        public async Task<IEnumerable<int>?> GetRunTagsAsync (
			Guid runId)
        {
			using IDbConnection conn = this._context.Database.GetDbConnection();
			conn.Open();

			return await conn.QueryAsync<int>(
				"SELECT TagId FROM " + SQL_Table_RunTags
                + " WHERE RunId = @RunId",
                new { RunId  = runId});
		}
		public async Task SaveRunTagsAsync(
			Guid runId,
            IEnumerable<int> tags)
		{
			using IDbConnection conn = this._context.Database.GetDbConnection();
			conn.Open();
            var tran = conn.BeginTransaction();

            if (tags == null) tags = new List<int>();

			try
            {
                // recupera le vecchie tag
                var oldTags = await conn.QueryAsync<int>(
				    "SELECT TagId FROM " + SQL_Table_RunTags
				    + " WHERE RunId = @RunId",
				    new { RunId = runId },
                    tran);
				if (oldTags == null) oldTags = new List<int>();

				await this.FindNewAsync(
                    oldTags,
                    tags,
                    (v) => v,
                    async (id) =>
                        await this.InsertTableAsync(
                            SQL_Table_RunTags,
                            conn, tran,
                            new { RunId = runId, TagId = id }
                            )
                    );


                // elimina i nodi da cancellare
                await this.FindDeletedAsync(
                    oldTags,
                    tags,
                    (v) => v,
                    async (id) =>
                        await this.DeleteTableAsync(
                            SQL_Table_RunTags,
                            conn, tran,
                            new { RunId = runId, TagId = id }
                            )
                    );
                
                tran.Commit();
                tran = null;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
		}

		private async Task AddToRecalcNeededAsync (
            IDbConnection conn,
            IDbTransaction tran,
            Guid? runId = null,
            int? calendarId = null,
            int? contractId = null,
            int? tagId = null
            )
        {
            await conn.ExecuteAsync(
            sql: "[dbo].[uo_RunsNeedRecalc]",
                param: new
                {
                    RunId = runId,
                    CalendarId = calendarId,
                    ContractId = contractId,
                    TagId = tagId
                },
                transaction: tran,
                commandType: CommandType.StoredProcedure
                );
        }

        #region carica un intera corsa
        private async Task<RunItem> InternalGetOneRunItemAsync(
            Guid runId,
            IDbConnection conn,
            IDbTransaction tran = null,
            bool loadLookups = true
            )
        {
            using var reader = await conn.QueryMultipleAsync(
                SQL_up_GetOneRunItem,
                transaction: tran,
                param: new { RunId = runId }, 
                commandType: CommandType.StoredProcedure);

            // Radice corse (una sola riga=
            var item = reader.Read<RunItem>().SingleOrDefault();
            if (item == null) return null;

            // varianti
            item.Variations = reader.Read<RunVariation>().ToList();
            // Periodi
            item.SubPeriods = reader.Read<RunPeriod>().ToList ();

            // Giorni addizionali
            item.AdditionalDays = reader.Read<RunAdditionalDay>().ToList();

            // calendari varianti
            #region variaton calendars
            var varCals = (from c in reader.Read<BlRunVarCalendars>()
                         group c by (Guid)c.RunVariationId into varCalendars
                         orderby varCalendars.Key
                         select varCalendars);
            foreach (var varCal in varCals)
            {
                var variatn = (from v in item.Variations
                               where v.RunVariationId == varCal.Key
                               select v).SingleOrDefault();
                if (variatn != null)
                {
                    variatn.Calendars = (from c in varCal
                                         select c.CalendarId)
                                         .ToList();
                }
            }
            #endregion

            #region nodi
            // nodi delle variatnti
            var nodes = (from n in reader.Read<BlRunNode>()
                         group n by (Guid) n.RunVariationId into varNodes
                         orderby varNodes.Key
                         select varNodes);
            foreach (var nodeGroup in nodes)
            {
                var variatn = (from v in item.Variations
                               where v.RunVariationId == nodeGroup.Key
                               select v).SingleOrDefault();
                if (variatn != null)
                {
                    variatn.Nodes = (from n in nodeGroup
                                     select new RunNode() 
                                     {
                                        RunNodeId = n.RunNodeId,
                                        Hour = n.Hour,
                                        ProgrNumber= n.ProgrNumber,
                                        CollectionPointId= n.CollectionPointId,
                                        CollectionPointData = new CollectionPointSimple ()
                                        {
                                            CollectionPointId = n.CollectionPointId,
                                            Description = n.CollectionPointDescription
                                        }
                                     })
                                     .ToList();
                }
            }
            #endregion

            #region mezzi
            // mezzi della corsa
            var periodCars = (from c in  reader.Read<BlRunPeriodCar>()
                         group c by c.RunPeriodId into cars
                         orderby cars.Key
                         select cars);
            foreach (var carGrouo in periodCars)
            {
                var period = (from p in item.SubPeriods
                              where p.RunPeriodId == carGrouo.Key
                              select p)
                              .FirstOrDefault();
                if (period != null)
                {
                    period.Cars = (from c in carGrouo select c as RunPeriodCar).ToList();
                }
            }

            //costi dei mezzi della corsa
            var carCosts= (from cc in  reader.Read<BlRunCarCost>()
                         group cc by cc.RunCarId into costs
                         orderby costs.Key
                         select costs);
            foreach (var costGroup in carCosts)
            {
                var car = item.SubPeriods.SelectMany(p => p.Cars.Where(c => c.RunCarId == costGroup.Key)).FirstOrDefault();
                if (car != null) 
                {
                    car.CarCosts = (from c in costGroup select c as RunCarCost).ToList();
                }
            }
            #endregion

            #region sostituzioni
            // sostituzioni
            var periodRepl = (from c in reader.Read<BlCarReplacement>()
                              group c by c.RunPeriodId into repl
                              orderby repl.Key
                              select repl);
            foreach (var replGroup in periodRepl)
            {
                var period = (from p in item.SubPeriods
                              where p.RunPeriodId == replGroup.Key
                              select p)
                              .SingleOrDefault();
                if (period != null)
                {
                    period.CarReplacements = (from r in replGroup select r as CarReplacement).ToList();
                }
            }
            // dettaglio sostituzioni
            var replDett = (from r in reader.Read<BlCarReplacementDetail>()
                            group r by r.CarReplacementId into repl
                            orderby repl.Key
                            select repl);
            foreach (var replGroup in replDett)
            {
                var repl = item.SubPeriods.SelectMany(p => p.CarReplacements.Where(r => r.CarReplacementId == replGroup.Key)).SingleOrDefault();
                if (repl != null)
                {
                    // crea le due liste da asegnare ai sostituiti e al sostituente
                    repl.OriginalPEriodCarIds = replGroup.Select(d => d.OriginaRunCarId).Distinct().ToList();
                    repl.ReplacedPEriodCarIds = replGroup.Select(d => d.ReplacedRunCarId).Distinct().ToList();
                }
            }
            #endregion

            #region sospensioni
            // Sospensioni (RunSuspensions=
            item.Suspensions = reader.Read<RunSuspension>().ToList();
            #endregion

            #region loockups
            if (loadLookups == true)
            {
                item.ContractData = reader.Read<Contract>().SingleOrDefault();

                // i calendari non vengono neanche caricati
                // var allCalendars = reader.Read<CalendarItem>().ToList();

                var allAssociates = reader.Read<Associate>().ToList();
                var allCars = reader.Read<Car>().ToList();

                // assegna i calendari a lle varianti
                // Non serve più perchè non engono neanche caricati
                /*
                foreach (var v in item.Variations)
                {
                    if (v.Calendars != null)
                    {
                        var calIds = (from c in v.Calendars
                                      select c.CalendarId);

                        // assegnare il valore a tutti i calendari figli
                        v.Calendars = allCalendars.Where(c => calIds.Contains(c.CalendarId)).ToList();
                    }
                }
                */

                // assegna le diette e i mezzi ai  perido cars
                if (item.SubPeriods != null)
                {
                    foreach (var p in item.SubPeriods)
                    {
                        if (p.Cars != null)
                        {
                            foreach (var pc in p.Cars)
                            {
                                pc.AssociateData = allAssociates.Where(a => a.AssociateId == pc.AssociateId).SingleOrDefault();
                                pc.CarData = allCars.Where(c => c.CarId == pc.CarId).SingleOrDefault();
                            }
                        }
                    }
                }
            }
            #endregion

            return item;
        }
        #endregion

        #region operazioni singole sulle parti

        #region runs
        private object GetRunData(
            RunItem runItem)
        {
            return new
            {
                runItem.StartDate,
                runItem.EndDate,
                runItem.ContractId,
                runItem.ContractRowNumber,
                runItem.Extra,
				runItem.RunName,

                runItem.Note
            };
        }
        private object GetRunKey(
            Guid runId)
        {
            return new
            {
                RunId = runId
            };
        }
        #endregion

        #region variatiants
        private object GetVariationData (
            RunVariation variation)
        {
            return new
            {
                variation.StartDate,

                StartTime = this.NormalizeTimeSpanValue(variation.StartTime),
                EndTime = this.NormalizeTimeSpanValue (variation.EndTime),

                variation.LineNumber,
                variation.RunNumber,

                variation.Path,
                variation.RequestedFrequency,

                variation.Km,
                variation.RequestedCapacity,

                variation.Monday,
                variation.Tuesday,
                variation.Wednesday,
                variation.Thursday,
                variation.Friday,
                variation.Saturday,
                variation.Sunday,

                variation.Note
            };
        }
        private object GetVariationKey(
            Guid runId,
            Guid runVariationId)
        {
            return new
            {
                RunId = runId,
                RunVariationId = runVariationId
            };
        }

        private async Task InsertVariationAsync(
            Guid runId,
            RunVariation variation,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            if (variation.RunVariationId == Guid.Empty) variation.RunVariationId = Guid.NewGuid();

            await this.InsertTableAsync(
                SQL_Table_RunVariations,
                conn, tran,
                this.JoinObjects (
                    this.GetVariationKey(runId, variation.RunVariationId),
                    this.GetVariationData(variation)
                    )
                );

            // inserisce i calendari
            if (variation.Calendars != null)
            {
                foreach (var cal in variation.Calendars)
                {
                    await this.InsertCalendarAsync(variation.RunVariationId, cal, conn, tran);
                }
            }


            // inserisce i nodi della variante
            #region inserisce tutti i nodi
            if (variation.Nodes != null)
            {
                foreach (var node in variation.Nodes)
                {
                    await this.InsertNodeAsync(variation.RunVariationId, node, conn, tran);
                }
            }
            #endregion
        }
        private async Task UpdateVariationAsync(
            Guid runId,
            RunVariation variation,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_RunVariations,
                conn, tran,
                this.GetVariationKey(runId, variation.RunVariationId),
                this.GetVariationData(variation)
                );
        }
        private async Task DeleteVariationAsync(
            Guid runId,
            Guid runVariationId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunVariations,
                conn, tran,
                this.GetVariationKey(runId, runVariationId)
                );
        }
        #endregion

        #region nodes
        private object GetNodeData(
            RunNode node)
        {
            return new
            {
                node.ProgrNumber,
                Hour = this.NormalizeTimeSpanValue( node.Hour),
                node.CollectionPointId
            };
        }
        private object GetNodeKey(
            Guid runVariationId,
            Guid runNodeId)
        {
            return new
            {
                RunVariationId = runVariationId,
                RunNodeId = runNodeId
            };
        }

        private async Task InsertNodeAsync(
            Guid runVariationId,
            RunNode node,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            if (node.RunNodeId == Guid.Empty) node.RunNodeId = Guid.NewGuid();

            await this.InsertTableAsync(
                SQL_Table_RunNodes,
                conn, tran,
                this.JoinObjects(
                    this.GetNodeKey(runVariationId, node.RunNodeId),
                    this.GetNodeData(node)
                    )
                );
        }
        private async Task UpdateNodeAsync(
            Guid runVariationId,
            RunNode node,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_RunNodes,
                conn, tran,
                this.GetNodeKey(runVariationId, node.RunNodeId),
                this.GetNodeData(node)
                );
        }
        private async Task DeleteNodeAsync(
            Guid runVariationId,
            Guid runNodeId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunNodes,
            conn, tran,
                this.GetNodeKey(runVariationId, runNodeId)
                );
        }
        #endregion

        #region calendari varinte
        private object GetCalendarKey(
            Guid runVariationId,
            int calendarID)
        {
            return new
            {
                RunVariationId = runVariationId,
                CalendarId = calendarID
            };
        }

        private async Task InsertCalendarAsync(
            Guid runVariationId,
            int calendarId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.InsertTableAsync(
                SQL_Table_RunCalendars,
                conn, tran,
                this.GetCalendarKey(runVariationId, calendarId)
                );
        }

        private async Task DeleteCalendarAsync(
            Guid runVariationId,
            int calendarId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunCalendars,
                conn, tran,
                this.GetCalendarKey(runVariationId, calendarId)
                );
        }
        #endregion

        #region periodi
        private object GetPeriodData(
            RunPeriod period)
        {
            return new
            {
                period.StartDate,
                period.EndDate,

                period.Monday,
                period.Tuesday,
                period.Wednesday,
                period.Thursday,
                period.Friday,
                period.Saturday,
                period.Sunday,

                period.Note
            };
        }
        private object GetPeriodKey(
            Guid runId,
            Guid runPeriodId)
        {
            return new
            {
                RunId = runId,
                RunPeriodId = runPeriodId
            };
        }

        private async Task InsertPeriodAsync(
            Guid runId,
            RunPeriod period,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            if (period.RunPeriodId == Guid.Empty) period.RunPeriodId = Guid.NewGuid();

            await this.InsertTableAsync(
                SQL_Table_RunPeriods,
                conn, tran,
                this.JoinObjects(
                    this.GetPeriodKey(runId, period.RunPeriodId),
                    this.GetPeriodData(period)
                    )
                );

            // aggiunge i mezzi del periodo
            if (period.Cars != null)
            {
                foreach (var runCar in period.Cars)
                {
                    await this.InsertRunCarAsync(period.RunPeriodId, runCar, conn, tran);
                }
            }
            // aggiunge le sostituzioni dei mezzi del periodo
            if (period.CarReplacements != null)
            {
                foreach (var replacement in period.CarReplacements)
                {
                    await this.InsertReplacementAsync(period.RunPeriodId, replacement, conn, tran);
                }
            }
        }
        private async Task UpdatePeriodAsync(
            Guid runId,
            RunPeriod period,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_RunPeriods,
                conn, tran,
                this.GetPeriodKey(runId, period.RunPeriodId),
                this.GetPeriodData(period)
                );
        }
        private async Task DeletePeriodAsync(
            Guid runId,
            Guid runPeriodId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunPeriods,
                conn, tran,
                this.GetPeriodKey(runId, runPeriodId)
                );
        }
        #endregion

        #region mezzi del periodo
        private object GetRunCarData(
            RunPeriodCar periodCar)
        {
            return new
            {
                periodCar.AssociateId,
                periodCar.CarId,
                periodCar.CarType,

                periodCar.Note
            };
        }
        private object GetRunCarKey(
            Guid runPeriodId,
            Guid runCarId)
        {
            return new
            {
                RunPeriodId = runPeriodId,
                RunCarId = runCarId
            };
        }

        private async Task InsertRunCarAsync(
            Guid runPeriodId,
            RunPeriodCar periodCar,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            if (periodCar.RunCarId == Guid.Empty) periodCar.RunCarId = Guid.NewGuid();

            await this.InsertTableAsync(
                SQL_Table_RunCars,
                conn, tran,
                this.JoinObjects(
                    this.GetRunCarKey(runPeriodId, periodCar.RunCarId),
                    this.GetRunCarData(periodCar)
                    )
                );

            // aggiunge i costi del mezzo
            if (periodCar.CarCosts != null)
            {
                foreach (var carCost in periodCar.CarCosts )
                {
                    await this.InsertCarCostAsync(periodCar.RunCarId, carCost, conn, tran );
                }
            }
        }
        private async Task UpdateRunCarAsync(
            Guid runPeriodId,
            RunPeriodCar periodCar,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_RunCars,
                conn, tran,
                this.GetRunCarKey(runPeriodId, periodCar.RunCarId),
                this.GetRunCarData(periodCar)
                );
        }
        private async Task DeleteRunCarAsync(
            Guid runPeriodId,
            Guid runCarId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunCars,
                conn, tran,
                this.GetRunCarKey(runPeriodId, runCarId)
                );
        }
        #endregion

        #region costi dei mezzi
        private object GetCarCostData(
            RunCarCost carCost)
        {
            return new
            {
                carCost.StartDate,

                carCost.KmPrice,
                carCost.KmPriceExtra,
                carCost.DayPrice,
                carCost.DayForfait,
                carCost.DayIntegration
            };
        }
        private object GetCarCostKey(
            Guid runCarId,
            Guid carCostId)
        {
            return new
            {
                RunCarId = runCarId,
                RunCarCostId = carCostId
            };
        }

        private async Task InsertCarCostAsync(
            Guid runCarId,
            RunCarCost carCost,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            if (carCost.RunCarCostId == Guid.Empty) carCost.RunCarCostId = Guid.NewGuid();

            await this.InsertTableAsync(
                SQL_Table_RunCarCosts,
                conn, tran,
                this.JoinObjects(
                    this.GetCarCostKey(runCarId, carCost.RunCarCostId),
                    this.GetCarCostData(carCost)
                    )
                );
        }
        private async Task UpdateCarCostAsync(
            Guid runCarId,
            RunCarCost carCost,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_RunCarCosts,
                conn, tran,
                this.GetCarCostKey(runCarId, carCost.RunCarCostId),
                this.GetCarCostData(carCost)
                );
        }
        private async Task DeleteCarCostAsync(
            Guid runCarId,
            Guid carCostId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunCarCosts,
                conn, tran,
                this.GetCarCostKey(runCarId, carCostId)
                );
        }
        #endregion

        #region sostituzioni dei mezzi
        private object GetReplacementData(
            CarReplacement replacement)
        {
            return new
            {
                replacement.StartDate,
                replacement.EndDate,
                replacement.Note
            };
        }
        private object GetReplacementKey(
            Guid runPeriodId,
            Guid carReplacementId)
        {
            return new
            {
                RunPeriodId = runPeriodId,
                CarReplacementId = carReplacementId
            };
        }
        private async Task InsertReplacementAsync(
            Guid runPeriodId,
            CarReplacement replacement,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            if (replacement.CarReplacementId == Guid.Empty) replacement.CarReplacementId = Guid.NewGuid();

            await this.InsertTableAsync(
                SQL_Table_CarReplacements,
                conn, tran,
                this.JoinObjects(
                    this.GetReplacementKey(runPeriodId, replacement.CarReplacementId),
                    this.GetReplacementData(replacement)
                    )
                );

            // aggiunge il dettaglio delle sostituazioni dei mezzi
            await this.UpdateReplacementDetailsAsync(replacement, conn, tran);
        }
        private async Task UpdateReplacementAsync(
            Guid runPeriodId,
            CarReplacement replacement,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_CarReplacements,
                conn, tran,
                this.GetReplacementKey(runPeriodId, replacement.CarReplacementId),
                this.GetReplacementData(replacement)
                );
        }
        private async Task DeleteReplacementAsync(
            Guid runPeriodId,
            Guid carReplacementId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_CarReplacements,
                conn, tran,
                this.GetReplacementKey(runPeriodId, carReplacementId)
                );
        }
        #endregion

        #region dettaglio sostitutzioni
        private object GetReplacementDetailDeleteKey(
            Guid carReplacementId)
        {
            return new
            {
                CarReplacementId = carReplacementId
            };
        }

        private async Task UpdateReplacementDetailsAsync (
            CarReplacement replacement,
            IDbConnection conn,
            IDbTransaction tran)
        {
            // elimina tutti i dettagli e li reinserisce
            await this.DeleteTableAsync(
                SQL_Table_CarReplacementDetails,
                conn, tran,
                this.GetReplacementDetailDeleteKey(replacement.CarReplacementId)
                );

            // reinserisce tutti i dati
            if (replacement.OriginalPEriodCarIds != null
                && replacement.OriginalPEriodCarIds.Count > 0
                && replacement.ReplacedPEriodCarIds != null
                && replacement.ReplacedPEriodCarIds.Count > 0
                )
            {
                foreach (var orgId in replacement.OriginalPEriodCarIds)
                {
                    foreach (var newId in replacement.ReplacedPEriodCarIds)
                    {
                        await this.InsertTableAsync(
                            SQL_Table_CarReplacementDetails,
                            conn, tran,
                            new
                            {
                                CarReplacementId = replacement.CarReplacementId,
                                OriginaRunCarId = orgId,
                                ReplacedRunCarId = newId
                            });
                    }
                }
            }
        }
        #endregion

        #region sospensioni
        private object GetSuspensionData(
            RunSuspension suspension)
        {
            return new
            {
                suspension.StartDate,
                suspension.EndDate,
                suspension.SuspensionTypeId,

                suspension.SuspensionNote
            };
        }
        private object GetSuspensionKey(
            Guid runId,
            Guid runSuspensionId)
        {
            return new
            {
                RunId = runId,
                RunSuspensionId = runSuspensionId
            };
        }

        private async Task InsertSuspensionAsync(
            Guid runId,
            RunSuspension suspension,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            if (suspension.RunSuspensionId == Guid.Empty) suspension.RunSuspensionId = Guid.NewGuid();

            await this.InsertTableAsync(
                SQL_Table_RunSuspensions,
                conn, tran,
                this.JoinObjects(
                    this.GetSuspensionKey(runId, suspension.RunSuspensionId),
                    this.GetSuspensionData(suspension)
                    )
                );
        }
        private async Task UpdateSuspensionAsync(
            Guid runId,
            RunSuspension suspension,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_RunSuspensions,
                conn, tran,
                this.GetSuspensionKey(runId, suspension.RunSuspensionId),
                this.GetSuspensionData(suspension)
                );
        }
        private async Task DeleteSuspensionAsync(
            Guid runId,
            Guid runSuspensionId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunSuspensions,
                conn, tran,
                this.GetSuspensionKey(runId, runSuspensionId)
                );
        }
        #endregion

        #region Gironi addizionali
        private object GetDayData(
            RunAdditionalDay addDay)
        {
            return new
            {
                addDay.Note
            };
        }
        private object GetDayKey(
            Guid runId,
            DateTime day)
        {
            return new
            {
                RunId = runId,
                Day = day
            };
        }

        private async Task InsertDayAsync(
            Guid runId,
            RunAdditionalDay addDay,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.InsertTableAsync(
                SQL_Table_RunAdditionalDays,
                conn, tran,
                this.JoinObjects(
                    this.GetDayKey(runId, addDay.Day),
                    this.GetDayData(addDay)
                    )
                );
        }
        private async Task UpdateDayAsync(
            Guid runId,
            RunAdditionalDay addDay,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Table_RunAdditionalDays,
                conn, tran,
                this.GetDayKey(runId, addDay.Day),
                this.GetDayData(addDay)
                );
        }
        private async Task DeleteDayAsync(
            Guid runId,
            DateTime day,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.DeleteTableAsync(
                SQL_Table_RunAdditionalDays,
                conn, tran,
                this.GetDayKey(runId, day)
                );
        }
        #endregion

        #endregion
    }
}