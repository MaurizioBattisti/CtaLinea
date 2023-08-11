using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface IRunRepository
    {
        Task<RunItem> GetOneRunItemAsync(Guid runId);

        Task DeleteRunAsync(
            Guid runId);
        Task SaveRuAsync(
            RunItem runItem);

		Task<IEnumerable<int>?> GetRunTagsAsync(
			Guid runId);
        Task SaveRunTagsAsync(
            Guid runId,
            IEnumerable<int> tags);

        Task<IDictionary<Guid, OperationResponse>> MultiRunAddSuspensionsAsync(
            IEnumerable<Guid> runIds,
            SetSuspensionRequest suspnesion
            );
        Task<Guid> CreateRunCopyAsync(
            CreateRunCopyRequest request);
        
        Task<bool> SaveRunInternalNoteAsync(
            Guid runId,
            InternalNoteQueryItem note);
    }
}