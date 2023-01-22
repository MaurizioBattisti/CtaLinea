using CtaLinea.Model.Helpers;
using CtaLinea.Model.Runs;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Services
{
    public interface ICompleteRunCheckerService
    {
        Task<RunCheckResult> CheckRunAsync(RunItem run);
    }
}