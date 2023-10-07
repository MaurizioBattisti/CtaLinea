using CtaLinea.Model.Filters;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Runs;
using CtaLinea.QueryModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IRunQueries
	{
		Task<QueryItemList<RunItemQueryModel>> GetRunListAsycn(
			IFilteringContext filterContext,
            RunAdvancedFilters advancedFilter = null);
		Task<RunItemQueryModel> GetOneRunAsync(
			Guid id);

		Task<QueryItemList<RunVariationQueryModel>> GetRunVariationsAsync(
			Guid runId,
			IFilteringContext filterContext);
        Task<RunVariationQueryModel> GetOneVariationAsync(
            Guid id);

		Task<InternalNoteQueryItem?> GetNoteAsync(
			Guid id);

        Task<IEnumerable<RunForPrintModel>> GetRunForPrintAsync(
            IEnumerable<int> runCtaIds,
            int? contractId,
            DateTime? startDate,
            DateTime? endDate);
    }
}
