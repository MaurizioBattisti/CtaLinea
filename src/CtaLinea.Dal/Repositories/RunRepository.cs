using CtaLinea.Model.Base;
using CtaLinea.Model.External;
using CtaLinea.Model.Runs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.Model.Runs;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class RunRepository 
        : IRunRepository
    {
        private const string SQL_up_GetOneRunItem = "[dbo].[up_GetOneRunItem]";

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

        public async Task<RunItem?> GetOneRunItemAsync(
            Guid runId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var reader = await conn.QueryMultipleAsync(
                SQL_up_GetOneRunItem,
                param: new { RunId = runId }, 
                commandType: CommandType.StoredProcedure);

            // Radice corse (una sola riga=
            var item = reader.Read<RunItem>().SingleOrDefault();
            if (item == null) return null;

            // varianti
            item.Variations = reader.Read<RunVariation>().ToList();
            // Periodi
            item.SubPeriods = reader.Read<RunPeriod>().ToList ();

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
                var car = item.SubPeriods.SelectMany(p => p.Cars.Where(c => c.CarId == costGroup.Key)).FirstOrDefault();
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
            item.ContractData = reader.Read<Contract>().SingleOrDefault();

            var allCalendars = reader.Read<Calendar>().ToList();
            var allAssociates = reader.Read<Associate>().ToList();
            var allCars = reader.Read<Car>().ToList();

            // assegna i calendari a lle varianti
            foreach (var v in item.Variations)
            {
                v.CalendarData = allCalendars.Where (c =>  c.CalendarId == v.CalendarId).SingleOrDefault();
            }

            // assegna le diette e i mezzi ai  perido cars
            foreach (var p in item.SubPeriods)
            {
                foreach (var pc in p.Cars)
                {
                    pc.AssociateData = allAssociates.Where(a => a.AssociateId == pc.AssociateId).SingleOrDefault();
                    pc.CarData = allCars.Where(c => c.CarId == pc.CarId).SingleOrDefault();
                }
            }
            #endregion

            return item;
        }

    }
}
