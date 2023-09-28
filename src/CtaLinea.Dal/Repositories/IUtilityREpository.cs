using CtaLinea.Model.QueryModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public interface IUtilityREpository
	{
		Task ChangeRunCarDataAsync(IDictionary<Guid, Guid> replacementMap, DateTime? refDate = null);
		Task<IEnumerable<Guid>> GetCarForDiscontinuationAsync(Guid associateId, DateTime? refDate = null);

        Task<IEnumerable<RunIncongruenceModel>> GetRunIncongruenceASync(
            Guid? runId = null,
            DateTime? startDate = null,
            DateTime? endDAte = null,
            IEnumerable<int> whatIncongruence = null
            );
        Task<IEnumerable<ElastibusDayExport>> GetElastibusDayForExportAsyunc(
            DateTime? startDate,
            DateTime? endDate,
            int? contractId
            );
        Task<IEnumerable<ExportRunTt>> GetExportTtAsync(
            DateTime? startDate,
            DateTime? endDate,
            int? contractId
            );

	}
}