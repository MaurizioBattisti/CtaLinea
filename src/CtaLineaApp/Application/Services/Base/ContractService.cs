
using CtaLinea.Model.Base;
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
    }
}
