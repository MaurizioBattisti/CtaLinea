using CtaLinea.Model.Runs;
using CtaLinea.Model.Base;

namespace CtaLineaApp.Application.Services.Run
{
    public class RunService 
        : IRunService
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
            var variation = new RunVariation()
            {
                RunVariationId = Guid.NewGuid(),
                StartDate = startDate,

                Nodes = new List<RunNode>()
            };
            if (run.Variations == null) run.Variations = new List<RunVariation>() { variation };
            else run.Variations.Add(variation);
            return variation;
        }

        public RunPeriod CreateNewPeriod(
            RunItem run,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var period = new RunPeriod()
            {
                RunPEriodId = Guid.NewGuid(),
                StartDate = startDate,
                EndDate = endDate,

                Cars = new List<RunPeriodCar>(),
                CarReplacements = new List<CarReplacement>()
            };

            // aggiunge i mezzi predefiniti vuoti
            this.CreatePeriodCar(period, CarTypeEnum.Primary);
            this.CreatePeriodCar(period, CarTypeEnum.Spare1);
            this.CreatePeriodCar(period, CarTypeEnum.Spare2);

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
                RunPeriodCarId = Guid.NewGuid(),

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
                StartDAte = startdate
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

    }
}
