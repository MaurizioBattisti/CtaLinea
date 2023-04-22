using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.QueryModel;
using CtaLineaApp.Application.Services.Helper;
using System.Net.NetworkInformation;

namespace CtaLineaApp.Application.Services.Base
{
    public class ContractService 
        : IContractService
    {
        private readonly IHttpService _http;

        public ContractService(
            IHttpService htto)
        {
            _http = htto;
        }

        public Contract? Current { get; set; }
        public OperationPeriodQueryItem? CurrentPEriod { get; set; }

        public async Task<ContractQueryItem?> GetOneAsync (int id)
        {
            var url = string.Format(Constants.Endpoint_OneContract_Fmt, id);
            var data = await this._http.Get<ContractQueryItem>(url);
            return data;
        }
        public async Task<int?> InsertAsync(Contract item)
        {
            return await this._http.Post<int, CheckResult>(Constants.Endpoint_Contracts, item);
        }
        public async Task<int?> UpdateAsync(Contract item)
        {
            var url = string.Format(Constants.Endpoint_OneContract_Fmt, item.ContractId);
            return await this._http.Put<int, CheckResult>(url, item);
        }
        public async Task DeleteAsync(int id)
        {
            var url = string.Format(Constants.Endpoint_OneContract_Fmt, id);
            await this._http.Delete(url);
        }
    }
}
