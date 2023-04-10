using CtaLinea.Model.Runs;
using CtaLinea.Model.Base;
using CtaLineaApp.Application.Model;
using System.Security.Cryptography;
using Radzen.Blazor.Rendering;
using System.Xml.Linq;
using System.Data.SqlTypes;

namespace CtaLineaApp.Application.Services.Run
{
    public class RunModelService 
        : IRunModelService
    {
        public RunItem CreateNewRun(
            Contract contract,
            Guid? nreRunId = null)
        {
            var run = new RunItem()
            {
                RunId = nreRunId ?? Guid.NewGuid(),
                ContractId = contract.ContractId,
                ContractData = contract,
                Extra = false,

                Variations = new List<RunVariation>(),
                SubPeriods = new List<RunPeriod>(),
                Suspensions = new List<RunSuspension>(),
                AdditionalDays = new List<RunAdditionalDay>(),
            };

            // aggiune la variante di default
            var variation = this.CreateNewVariation(run);
            
            return run;
        }
        public RunVariation CreateNewVariation(
            RunItem run,
            DateTime? startDate = null)
        {
            RunVariation variation;

			if (startDate == null
                || run.Variations == null
                || run.Variations.Count == 0)
            {
                variation = new RunVariation()
                {
                    RunVariationId = Guid.NewGuid(),
                    StartDate = startDate,

                    Calendars = new List<int> (),
                    Nodes = new List<RunNode>()
                };
            }
            else
            {
                var oldVar = (from v in run.Variations
                              where v.StartDate < startDate
                              orderby v.StartDate descending
                              select v)
                              .FirstOrDefault ();
                
                // se non ne trova una con la data cerca la variante di default
                if(oldVar == null)
                {
					oldVar = (from v in run.Variations
								  where v.StartDate == null
								  select v)
								  .SingleOrDefault();
				}

				if (oldVar != null)
                {
                    variation = this.CloneVariation(oldVar);
                    variation.RunVariationId = Guid.NewGuid();
                    variation.StartDate= startDate;
				}
				else
                {
                    variation = new RunVariation()
                    {
                        RunVariationId = Guid.NewGuid(),
                        StartDate = startDate,

                        Nodes = new List<RunNode>()
                    };
                }
			}

			if (run.Variations == null) run.Variations = new List<RunVariation>() { variation };
            else run.Variations.Add(variation);
            return variation;
        }

        public RunNode CreateNewNode (
			RunVariation variation,
            bool addToList = true)
        {
            RunNode? lastNode = null;

			if (variation.Nodes != null)
            {
                lastNode = (from n in variation.Nodes
                            orderby n.ProgrNumber descending, n.Hour descending
							select n)
                            .FirstOrDefault();
            }

            var newHour = new TimeSpan(8, 1, 0);
            if (lastNode != null)
            {
                newHour = lastNode.Hour.Add(new TimeSpan(0, 5, 1));
			}

            var node = new RunNode()
            {
                RunNodeId = Guid.NewGuid(),
                ProgrNumber = lastNode?.ProgrNumber + 1  ?? 1,
                Hour = newHour
			};
            if (addToList == true) 
            {
                this.AddNodeToVariation(variation, node); 
            }

			return node;
		}
        public void AddNodeToVariation (
			RunVariation variation,
            RunNode node)
        {
			if (variation.Nodes == null)
			{
				variation.Nodes = new List<RunNode>() { node };
			}
			else
			{
				variation.Nodes.Add(node);
			}
		}
        public void RemoveNode (
			RunVariation variation,
			RunNode node)
        {
            if (variation.Nodes == null) return;
			variation.Nodes.Remove(node);
		}
		public void ReorderNodes (
			RunVariation variation)
		{
            int progr = 0;
			if (variation.Nodes != null)
			{
				var list = (from n in variation.Nodes
							orderby n.Hour ascending, n.ProgrNumber ascending
							select n)
							.ToList ();
                foreach (var n in list)
                {
                    n.ProgrNumber = ++progr;
				}
			}
		}

		public RunPeriod CreateNewPeriod(
            RunItem run,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var period = new RunPeriod()
            {
                RunPeriodId = Guid.NewGuid(),
                StartDate = startDate,
                EndDate = endDate,

                Cars = new List<RunPeriodCar>(),
                CarReplacements = new List<CarReplacement>()
            };

            // aggiunge i mezzi predefiniti vuoti
            this.CreatePeriodCar(period, CarTypeEnum.Primary);
            this.CreatePeriodCar(period, CarTypeEnum.Spare);

            if (run.SubPeriods == null) run.SubPeriods = new List<RunPeriod>() { period };
            else run.SubPeriods.Add(period);
            return period;
        }

        public RunPeriodCar CreatePeriodCar(
            RunPeriod period,
            CarTypeEnum runCarType = CarTypeEnum.Primary,
            Guid? associateId = null,
            Guid? carId = null
            )
        {
            var periodCar = new RunPeriodCar()
            {
                RunCarId = Guid.NewGuid(),

                RunCarType = runCarType,
                AssociateId = associateId ?? Guid.Empty,
                CarId = carId ?? Guid.Empty,

                CarCosts = new List<RunCarCost>()
            };

            // add default cost
            this.CreateRunCarCost(periodCar);

            if (period.Cars == null) period.Cars = new List<RunPeriodCar>() { periodCar };
            else period.Cars.Add(periodCar);
            return periodCar;
        }

        public RunCarCost? CreateRunCarCost(
            RunPeriodCar periodCar,
            DateTime? startdate = null,
            bool add = true)
        {
            if (periodCar.RunCarType != CarTypeEnum.Primary
                && periodCar.RunCarType != CarTypeEnum.Replacement)
            {
                return null;
            }

            // recupera i costi e li propone sul nuovo costo
            RunCarCost oldCost = new RunCarCost();
            if (periodCar.CarCosts != null
                && startdate != null)
            {
                var mayBeCost = (from cc in periodCar.CarCosts
                                 where cc.StartDate == null
                                 || cc.StartDate <= startdate
                                 orderby cc.StartDate descending
                                 select cc).FirstOrDefault();
                if (mayBeCost != null)
                {
                    oldCost = mayBeCost;
                }
            }

            var cost = new RunCarCost()
            {
                RunCarCostId = Guid.NewGuid(),
                StartDate = startdate,

                KmPrice = oldCost.KmPrice,
                KmPriceExtra= oldCost.KmPriceExtra,
                DayPrice = oldCost.DayPrice,
                DayForfait = oldCost.DayForfait,
                DayIntegration = oldCost.DayIntegration
            };

            if (add == true)
            {
                if (periodCar.CarCosts == null) periodCar.CarCosts = new List<RunCarCost>() { cost };
                else periodCar.CarCosts.Add(cost);
            }

            return cost;
        }

        public RunCarCost ComputeReplacementCost(
			RunPeriod period,
			CarReplacement currRepl
			)
		{
			var cost = new RunCarCost()
			{
				RunCarCostId = Guid.NewGuid(),
				StartDate = null
			};

			// corregge il vaore dei costi
			if (period.Cars != null
				&& currRepl.OriginalPEriodCarIds != null)
			{
				var orCarCost = period.Cars.Where(c => currRepl.OriginalPEriodCarIds.Contains(c.RunCarId))
								.SelectMany(c => c.CarCosts ?? new List<RunCarCost>());

				// calcola il totale dei costi dei mezzi originali
				var orgCosts = (from cc in orCarCost
								where cc.StartDate == null
								group cc by cc.StartDate into tot
								select new RunCarCost()
								{
									StartDate = tot.Key,
									KmPrice = tot.Sum(x => x.KmPrice),
									KmPriceExtra = tot.Sum(x => x.KmPriceExtra),
									DayPrice = tot.Sum(x => x.DayPrice),

									DayForfait = tot.Sum(x => x.DayForfait),
									DayIntegration = tot.Sum(x => x.DayIntegration)
								}).FirstOrDefault();

				if (orgCosts != null)
				{
					cost.KmPrice = orgCosts.KmPrice;
					cost.KmPriceExtra = orgCosts.KmPriceExtra;
					cost.DayPrice = orgCosts.DayPrice;
					cost.DayForfait = orgCosts.DayForfait;
					cost.DayIntegration = orgCosts.DayIntegration;
				}

				if (currRepl.ReplacedPEriodCarIds != null)
				{
					var newCarCost = period.Cars.Where(c => currRepl.ReplacedPEriodCarIds.Contains(c.RunCarId))
									.SelectMany(c => c.CarCosts ?? new List<RunCarCost>());

					// calcola il  totale dei mezzi già sostituiti
					var myCost = (from cc in newCarCost
								  where cc.StartDate == null
								  group cc by cc.StartDate into tot
								  select new RunCarCost()
								  {
									  StartDate = tot.Key,
									  KmPrice = tot.Sum(x => x.KmPrice),
									  KmPriceExtra = tot.Sum(x => x.KmPriceExtra),
									  DayPrice = tot.Sum(x => x.DayPrice),

									  DayForfait = tot.Sum(x => x.DayForfait),
									  DayIntegration = tot.Sum(x => x.DayIntegration)
								  }).FirstOrDefault();

					// sottrai il totale dei mezzi già sostituitoi da quello  degli originali
					if (myCost != null)
					{
						cost.KmPrice -= myCost.KmPrice;
						cost.KmPriceExtra -= myCost.KmPriceExtra;
						cost.DayPrice -= myCost.DayPrice;
						cost.DayForfait = (cost.DayForfait ?? decimal.Zero) - (myCost.DayForfait ?? decimal.Zero);
						cost.DayIntegration = (cost.DayIntegration ?? decimal.Zero) - (myCost.DayIntegration ?? decimal.Zero);
					}
				}
			}

			return cost;
		}

        public RunPeriodCar AddReplacmeent (
			RunPeriod period,
			RunPeriodCar car,
            CarReplacement currRepl
			)
		{
			if (currRepl.ReplacedPEriodCarIds == null)
			{
				currRepl.ReplacedPEriodCarIds = new List<Guid>();
			}
			currRepl.ReplacedPEriodCarIds.Add(car.RunCarId);
            return car;
		}

		public RunVariation? GetDefaultVariation (
            RunItem run)
        {
            RunVariation? variation = null;

            if (run.Variations != null)
            {
                variation = (from v in run.Variations
                             where v.StartDate == null
                             select v)
                             .FirstOrDefault();
                if (variation == null)
                {
                    variation = (from v in run.Variations
                            orderby v.StartDate ascending
                            select v)
                            .FirstOrDefault();
                }
            }
            return variation;
        }

        public IList<RunPeriodGroup> GetPeriodGroups (
            RunItem run)
        {
            var list = new List<RunPeriodGroup>();
            if (run.SubPeriods != null)
            {
                list = (from p in run.SubPeriods
                        group p by new { p.StartDate, p.EndDate } into g
                        select new RunPeriodGroup
                        {
                            StartDate = g.Key.StartDate,
                            EndDate = g.Key.EndDate,
                            SubPeriods = g.ToList()
                        }).OrderBy(p => p.StartDate)
						.ToList();

                /*                
                list = (from p in run.SubPeriods
                            select new RunPeriodGroup () { StartDate = p.StartDate, EndDate = p.EndDate }
                             ).Distinct ()
                             .OrderBy (p => p.StartDate )
                             .ToList ();
                */
            }

            return list;
        }

		public DateTime? GetMinValidDate(
	        RunItem runItem,
            DateTime? defaultDate,
            RunPeriod? period = null)
        {
            return this.Coalesce (period?.StartDate,
                runItem.StartDate, runItem?.ContractData?.StartDate, defaultDate);
		}
		public DateTime? GetMaxValidDate(
			RunItem runItem,
			DateTime? defaultDate,
            RunPeriod? period = null)
        {
			return this.Coalesce(period?.EndDate,
                runItem.EndDate, runItem?.ContractData?.EndDate, defaultDate);
		}

		#region funzioni private di clonazione
		private RunVariation CloneVariation (RunVariation source)
        {
            var variation = new RunVariation()
            {
                RunVariationId = source.RunVariationId,
                StartDate = source.StartDate,

                Monday = source.Monday,
                Tuesday = source.Tuesday,
                Wednesday = source.Wednesday,
                Thursday = source.Thursday,
                Friday = source.Friday,
                Saturday = source.Saturday,
                Sunday = source.Sunday,

                StartTime = source.StartTime,
                EndTime = source.EndTime,
                LineNumber = source.LineNumber,
                RunNumber = source.RunNumber,
                Km = source.Km,
                RequestedFrequency = source.RequestedFrequency,
                Path = source.Path,
                RequestedCapacity = source.RequestedCapacity,
                Note = source.Note,

                Calendars = (source.Calendars == null ?
                        new List<int>()
                        : new List<int>(source.Calendars)),
                Nodes = new List<RunNode>()
			};

			if (source.Nodes != null)
			{
				var nodes = (from n in source.Nodes
							 select n.GetClone())
						 .ToList();
                foreach (var n in nodes)
                {
                    n.RunNodeId = Guid.NewGuid();
                }
                variation.Nodes = nodes;
			}

            return variation;
		}
        #endregion

        #region funzioni helper di gestione di dati
        private T? Coalesce<T> (params T?[] items)
        {
			T? value = default;
            foreach (var item in items)
            {
                if (item != null) 
                {
                    value = item; 
                    break; 
                }
            }
            return value;
        }

        #endregion
    }
}
