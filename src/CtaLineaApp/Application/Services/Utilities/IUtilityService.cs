using CtaLinea.Model.Utilities;

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

	}
}