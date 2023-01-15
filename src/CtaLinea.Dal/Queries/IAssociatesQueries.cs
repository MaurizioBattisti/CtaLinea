using System;
using System.Threading.Tasks;
using CtaLinea.Model.QueryModel;
using CtaLinea.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IAssociatesQueries
    {
        Task<QueryItemList<AssociateQueryItem>> GetAssociateListAsync(
            IFilteringContext filterContext);
        Task<AssociateQueryItem> GetOneAssociateAsync(
            Guid id);

        Task<QueryItemList<CarQueryItem>> GetCarListAsync(
            IFilteringContext filterContext);
        Task<QueryItemList<CarQueryItem>> GetAssociateCarListAsync(
            IFilteringContext filterContext,
            Guid associateId);
        Task<CarQueryItem> GetOneCarAsync(
            Guid id);

        Task<QueryItemList<DriverQueryItem>> GetDriverListAsync(
            IFilteringContext filterContext);
        Task<QueryItemList<DriverQueryItem>> GetAssociateDriverListAsync(
            IFilteringContext filterContext,
            Guid associateId);
        Task<DriverQueryItem> GetOneDriverAsync(
            Guid id);
    }
}