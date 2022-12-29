using CtaLinea.Model.QueryModel;
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
			IFilteringContext filterContext);
		Task<RunItemQueryModel> GetOneRunAsync(
			Guid id);

		Task<QueryItemList<RunVariationQueryModel>> GetRunVariationsAsync(
			Guid runId,
			IFilteringContext filterContext);
        Task<RunVariationQueryModel> GetOneVariationAsync(
            Guid id);
    }
}
