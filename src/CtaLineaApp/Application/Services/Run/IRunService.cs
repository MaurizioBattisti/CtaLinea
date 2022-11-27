using CtaLinea.Model.Base;
using CtaLinea.Model.Runs;

namespace CtaLineaApp.Application.Services.Run
{
    public interface IRunService
    {
        RunPeriod CreateNewPeriod(RunItem run, DateTime? startDate = null, DateTime? endDate = null);
        RunItem CreateNewRun(Contract contract);
        RunVariation CreateNewVariation(RunItem run, DateTime? startDate = null);
        RunPeriodCar CreatePeriodCar(RunPeriod period, CarTypeEnum runCarType = CarTypeEnum.Primary, Guid? associateId = null, Guid? carId = null);
        RunCarCost? CreateRunCarCost(RunPeriodCar periodCar, DateTime? startdate = null);

        RunVariation? GetDefaultVariation(
            RunItem run);
    }
}