using System;
using System.Threading.Tasks;
using CtaLinea.Model.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface ICollectionPointsQueries
    {
        Task<QueryItemList<CollectionPointQueryItem>> GetCollectionPointListAsync(
            IFilteringContext filterContext);

        Task<CollectionPointQueryItem> GetOneCollectionPointAsync(string id);
    }
}