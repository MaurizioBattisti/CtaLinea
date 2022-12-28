using CtaLinea.Model.Runs;
using CtaLinea.Model.Base;
using CtaLineaApp.Application.Model;
using System.Security.Cryptography;
using Radzen.Blazor.Rendering;
using System.Xml.Linq;

namespace CtaLineaApp.Application.Services.Run
{
    public class RunModelService 
        : IRunModelService
    {
        public RunItem CreateNewRun(
            Contract contract)
        {
            var run = new RunItem()
            {
                RunId = Guid.NewGuid(),
                ContractId = contract.ContractId,
                Extra = false,

                Variations = new List<RunVariation>(),
                SubPeriods = new List<RunPeriod>(),
                Suspensions = new List<RunSuspension>()
            };

            // aggiune la variante di default
            this.CreateNewVariation(run);
            // crea il periodo di default
            this.CreateNewPeriod(run);

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
            DateTime? startdate = null)
        {
            if (periodCar.RunCarType != CarTypeEnum.Primary
                || periodCar.RunCarType != CarTypeEnum.Replacement)
            {
                return null;
            }

            var cost = new RunCarCost()
            {
                RunCarCostId = Guid.NewGuid(),
                StartDate = startdate
            };

            if (periodCar.CarCosts == null) periodCar.CarCosts = new List<RunCarCost>() { cost };
            else periodCar.CarCosts.Add(cost);

            return cost;
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
                            select new RunPeriodGroup () { StartDate = p.StartDate, EndDate = p.EndDate }
                             ).Distinct ()
                             .OrderBy (p => p.StartDate )
                             .ToList ();
            }

            return list;
        }
		#region funzioni private di clonazione
		private RunVariation CloneVariation (RunVariation source)
        {
			var variation = new RunVariation()
			{
				RunVariationId = source.RunVariationId,
				StartDate = source.StartDate,
                CalendarId= source.CalendarId,
                CalendarData= source.CalendarData,

                Monday = source.Monday,
                Tuesday = source.Tuesday,
                Wednesday = source.Wednesday,
                Thursday = source.Thursday,
                Friday = source.Friday,
                Saturday = source.Saturday,
                Sunday = source.Sunday,

                StartTime=source.StartTime,
                EndTime =source.EndTime,
                LineNumber = source.LineNumber,
                RunNumber= source.RunNumber,
                Km= source.Km,
                RequestedFrequency = source.RequestedFrequency,
                Note= source.Note,

                Nodes = new List<RunNode>()
			};

			if (source.Nodes != null)
			{
				var nodes = (from n in source.Nodes
							 select n.GetClone())
						 .ToList();
                
                variation.Nodes = nodes;
			}

            return variation;
		}
		#endregion
	}
}
