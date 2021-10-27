using System;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IImportQueries
    {
        Task<QueryItemList<ImportQueryItem>> GetImportListAsync(
            IFilteringContext filterContext);
        Task<ImportQueryItem> GetOneImportAsync(
            Guid id);
        Task<ImportQueryItem> GetLastPendingImportAsync(
            string importDescr);

        Task<QueryItemList<ImportDetailQueryItem>> GetImportDetailListAsync(
            Guid importId, 
            IFilteringContext filterContext);
        Task<ImportDetailQueryItem> GetOneImportDetailAsync(
            Guid importId, 
            int serviceId);
    }
}