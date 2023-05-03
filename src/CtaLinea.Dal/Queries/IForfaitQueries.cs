using CtaLinea.Model.QueryModel;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IForfaitQueries
    {
        Task<QueryItemList<ForfaitQueryItem>> GetForfaitListAsync(IFilteringContext filterContext);
        Task<ForfaitQueryItem> GetOneForfaitAsync(int id);
    }
}