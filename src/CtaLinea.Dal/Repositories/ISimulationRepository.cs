using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using System.Data;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface ISimulationRepository
    {
        Task<SimulationStatusResponse> GetSimulationStatus(SimulationRequest request);
        Task ApplyChangeslSimulationAsync(
            ApplyChangesSimulationRequest applyRe);
        Task ApplyKmTotalSimulationAsync(
            KmToMatchCarRequest matchRequest,
            ApplyChangesSimulationRequest applyRe);

        Task SetDayAmountsAsync(
            SimulationSetDayAmountsRequest request);
        Task FinalizeAsync(
            FinalizeSimulationRequest request);
    }
}