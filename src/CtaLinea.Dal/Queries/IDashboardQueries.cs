using CtaLinea.Model.Dashboards;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IDashboardQueries
    {
        Task<CarCountResult> GetCarCountAsync(CarCountRequest request);
        Task<IEnumerable<KmDataItem>> GetKmAsync(KmDataItemRequest request);
        Task<IEnumerable<CostDataItem>?> GetCostsAsync(
            CostDataItemRequest request);

	}
}