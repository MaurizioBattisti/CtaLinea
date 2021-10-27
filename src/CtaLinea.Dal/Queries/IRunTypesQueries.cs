using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IRunTypesQueries
    {
        Task<QueryItemList<RunTypeQueryItem>> GetRunTypeListAsync(
            IFilteringContext filterContext);
        Task<RunTypeQueryItem> GetOneRunTypeAsync(
            string id);
    }
}