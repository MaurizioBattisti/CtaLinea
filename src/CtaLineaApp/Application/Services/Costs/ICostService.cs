using CtaLinea.Model.Costs;

namespace CtaLineaApp.Application.Services.Costs
{
    public interface ICostService
    {
        Task<IEnumerable<CostsByAssociate>> GetCostsByAssociateAsync(
            int? contractId = null, 
            DateTime? startDate = null, DateTime? endDate = null, 
            Guid? associateId = null, Guid? carId = null, 
            Guid? runId = null);

        Task<IEnumerable<CostByAssociateItem>?> GetCostsByAssociateASync(
            CalcCostsRequest request);
        Task<IEnumerable<CostByAssociateByRunItem>?> GetCostsByAssociateByRunASync(
            CalcCostsRequest request);
		Task<IEnumerable<CostByRunItem>?> GetCostsByRunASync(
            CalcCostsRequest request);
    }
}