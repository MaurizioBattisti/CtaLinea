using CtaLinea.Model.Checks;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Reports;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using CtaLinea.Model.Utilities;
using static System.Net.WebRequestMethods;

namespace CtaLineaApp.Application.Services.Utilities
{
    public interface IUtilityService
    {
        Task<IEnumerable<CarPlanningItem>> GetCarPlanningAsync(Guid? associateId = null, Guid? carId = null, DateTime? startDate = null, DateTime? endDate = null);
		Task<IEnumerable<RunPlanningItem>> GetRunPlanningAsync(
			Guid runId,
			DateTime? startDate = null,
			DateTime? endDate = null
			);
		Task<IEnumerable<OverlappingCarItem>> GetOverlappingCarsAsync(
			Guid runCarId,
			DateTime? startDate = null,
			DateTime? endDate = null
			);
		Task<IEnumerable<GlobalCarOverlappingItem>> GetGlobalOverlappingCarsAsync(
			DateTime? startDate = null,
			DateTime? endDate = null
			);

        Task<IEnumerable<Guid>?> GetCarsForDiscontinuationAsunc(
			Guid associateId,
			DateTime? refDate);
		Task ReplaceCarsAsync(
			IDictionary<Guid, Guid> carMap,
			DateTime? refDate);

		Task<IEnumerable<RunIncongruenceModel>?> GetRunIncongruenceAsync(
			RinIncongruenceRequest request);

		Task<OperationResponse?> AddElastibusDaysAsync(
			AddElastibusDaysRequest request);

		Task<IEnumerable<string>?> GetSimulationNamesAsync();
		Task DeleteSimulationAsync(
			string simulationName);
		Task<SimulationStatusResponse?> GetSimulationStatusAsync(
			SimulationRequest request);
		Task SimulationApplyChangesASync(
			ApplyChangesSimulationRequest request);
		Task SimulationKmTotalApplyChangesASync(
			ApplyChangeToSimRequest<KmToMatchCarRequest> request);
		Task FinalizeSimulationAsync(
			FinalizeSimulationRequest request);
		Task SimulationSetDaysAmountAsync(
			SimulationSetDayAmountsRequest request);

		Task<IEnumerable<NegativeKmItem>?> GetNegativeKmReportAsync(
			NegativeKmReportRequest request);
    }
}