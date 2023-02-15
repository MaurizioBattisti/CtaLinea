using CtaLinea.Model.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IUtilityQueries
    {
        Task<IEnumerable<CarPlanningItem>> GetCarPlanningAsync(Guid? associateId = null, Guid? carId = null, DateTime? startDate = null, DateTime? endDate = null);
		Task<IEnumerable<RunPlanningItem>> GetRunPlanningAsync(
			Guid runId,
			DateTime? startDate = null,
			DateTime? endDate = null
			);
	}
}