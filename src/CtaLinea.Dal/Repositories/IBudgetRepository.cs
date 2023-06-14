using CtaLinea.Model.Costs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<CostByAssociateItem>> GEtCostsByAssociateAsync(int? budgetId, CalcCostsRequest request);
        Task<IEnumerable<CostByRunItem>> GetRunCostAsync(CalcCostsRequest request);
        Task UpdateBudgetDetaulsAsync(CalcCostsRequest request);
    }
}