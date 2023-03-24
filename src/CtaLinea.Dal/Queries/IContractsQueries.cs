using System.Threading.Tasks;
using CtaLinea.Model.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IContractsQueries
    {
        Task<QueryItemList<ContractQueryItem>> GetContractListAsync(
            IFilteringContext filterContext);
        Task<ContractQueryItem> GetOneContractAsync(
            int id);

        Task<QueryItemList<OperationPeriodQueryItem>> GePOperatingPeriodstListAsync(
            IFilteringContext filterContext);
    }
}