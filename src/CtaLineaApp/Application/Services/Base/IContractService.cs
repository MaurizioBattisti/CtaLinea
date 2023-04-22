using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.QueryModel;

namespace CtaLineaApp.Application.Services.Base
{
    public interface IContractService
    {
        Contract? Current { get; set; }
        OperationPeriodQueryItem? CurrentPEriod { get; set; }

        Task<ContractQueryItem?> GetOneAsync(int id);
        Task<int?> InsertAsync(Contract item);
        Task<int?> UpdateAsync(Contract item);
        Task DeleteAsync(int id);
    }
}