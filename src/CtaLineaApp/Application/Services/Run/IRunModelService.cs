using CtaLinea.Model.Base;
using CtaLinea.Model.Runs;
using CtaLineaApp.Application.Model;

namespace CtaLineaApp.Application.Services.Run
{
    public interface IRunModelService
    {
        RunPeriod CreateNewPeriod(RunItem run, DateTime? startDate = null, DateTime? endDate = null);
        RunItem CreateNewRun(
            Contract contract,
            Guid? nreRunId = null);

        RunVariation CreateNewVariation(RunItem run, DateTime? startDate = null);
        RunPeriodCar CreatePeriodCar(RunPeriod period, CarTypeEnum runCarType = CarTypeEnum.Primary, Guid? associateId = null, Guid? carId = null);
        RunCarCost ComputeReplacementCost(
            RunPeriod period,
            CarReplacement currRepl
            );
		RunPeriodCar AddReplacmeent(
            RunPeriod period,
            RunPeriodCar car,
            CarReplacement currRepl
            );
		RunCarCost? CreateRunCarCost(RunPeriodCar periodCar, 
            DateTime? startdate = null,
			bool add = true);

        RunVariation? GetDefaultVariation(
            RunItem run);
        IList<RunPeriodGroup> GetPeriodGroups(
            RunItem run);
        
        RunNode CreateNewNode(
            RunVariation variation,
            bool addToList = true);
        void AddNodeToVariation(
            RunVariation variation,
            RunNode node);
        void RemoveNode(
            RunVariation variation,
            RunNode node);

		void ReorderNodes(
            RunVariation variation);

        DateTime? GetMinValidDate(
            RunItem runItem,
            DateTime? defaultDate,
            RunPeriod? period = null);
        DateTime? GetMaxValidDate(
            RunItem runItem,
            DateTime? defaultDate,
            RunPeriod? period = null);

    }
}