
using CtaLinea.Model.Base;

namespace CtaLineaApp.Application.Services.Base
{
    public class ContractService 
        : IContractService
    {
        public ContractService()
        {
            this.Current = new Contract();
        }

        public Contract? Current { get; set; }
    }
}
