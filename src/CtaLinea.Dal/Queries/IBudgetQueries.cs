using CtaLinea.Model.QueryModel;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
	public interface IBudgetQueries
	{
		Task<QueryItemList<BudgetQueryItem>> GetBudgetListAsync(IFilteringContext filterContext);
		Task<BudgetQueryItem> GetOneBudgetAsync(
			IFilteringContext filterContext,
			int id);
	}
}