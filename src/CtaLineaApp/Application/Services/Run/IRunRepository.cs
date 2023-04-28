using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Runs;
using static System.Net.WebRequestMethods;

namespace CtaLineaApp.Application.Services.Run
{
    public interface IRunRepository
    {
        Task<RunItem?> GetOneAsync(Guid runId);
        Task<RunItemQueryModel?> GetOneDetailAsync(
            Guid runId);

		Task SaveRunAsync(Guid runId, RunItem model);
        Task AddNewOneAsync(RunItem model);
        Task DeleteRunAsync(
            Guid runId);

        Task<IEnumerable<int>?> GetRunTagsAsync(
            Guid runId);
        Task SaveRunTagsAsync(
            Guid runId,
            IEnumerable<int> tags);

        Task<IEnumerable<RunNode>?> DecodeNodeAsync(string text);
	}
}