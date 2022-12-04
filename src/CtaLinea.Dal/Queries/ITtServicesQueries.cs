using System.Threading.Tasks;
using CtaLinea.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface ITtServicesQueries
    {
        Task<TtServiceQueryItem> GetOneTtServiceAsync(int id);
        Task<QueryItemList<TtServiceQueryItem>> GetTtServiceListAsync(IFilteringContext filterContext);
    }
}