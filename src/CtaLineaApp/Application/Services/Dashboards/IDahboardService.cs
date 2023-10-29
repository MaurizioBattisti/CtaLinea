using CtaLinea.Model.Dashboards;

namespace CtaLineaApp.Application.Services.Dashboards
{
    public interface IDahboardService
    {
        Task<CarCountResult?> GetCarCountAsync(CarCountRequest request);
        Task<IEnumerable<KmDataItem>?> GetKmAsync(KmDataItemRequest request);
        Task<IEnumerable<CostDataItem>?> GetCostsAsync(
            CostDataItemRequest request);

	}
}