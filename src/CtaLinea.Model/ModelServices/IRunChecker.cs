using CtaLinea.Model.Helpers;
using CtaLinea.Model.Runs;

namespace CtaLinea.Model.ModelServices
{
    public interface IRunChecker
    {
        Task<RunItem> CleanGraphAsync(RunItem run);
        Task<RunCheckResult> CheckRunAsync(RunItem run);
    }
}