using CtaLinea.Model.QueryModel;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IUsersQueries
    {
        Task<QueryItemList<UserQueryModel>> GetListAsync(IFilteringContext filterContext);
        Task<UserQueryModel> GetOneASync(string id);
    }
}