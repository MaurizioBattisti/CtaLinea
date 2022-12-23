using CtaLinea.Model.Runs;

namespace CtaLineaApp.Application.Services.Run
{
    public interface IRunRepository
    {
        Task<RunItem?> GetOneAsync(Guid runId);
    }
}