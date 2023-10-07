using CtaLinea.Model;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
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

        Task<MultiRunOperationResponse?> SetRunsSuspensionsAsync(
            MultiRunSetSuspensionRequest request);

        Task<OperationResult<Guid>?> CreateRunCopyAsync(
            CreateRunCopyRequest copyRequest
            );

        Task<InternalNoteQueryItem?> GetRunNoteAsync(Guid runId);

        Task<OperationResult<bool>?> UpdateRunInternalNoteAsync(
            Guid runId,
            InternalNoteQueryItem note);
        Task<IEnumerable<RunForPrintModel>?> GetRunsForPrintAsync(
            GetRunForPrintRequest request);

    }
}