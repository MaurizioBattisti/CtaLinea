
using CtaLinea.Model.Base;

namespace CtaLineaApp.Application.Services.Base
{
    public class ContractService 
        : IContractService
    {
        private readonly IList<Contract> _list;

        public ContractService()
        {
            this.Current = new Contract() { ContractId = 1, ContractDescription = "Appalto 2022 / 2026" , StartDate= new DateTime(2022, 9, 1), EndDate=new DateTime(2026, 8, 31) };
            _list = new List<Contract>()
            {
                Current
            };
        }

        public Contract? Current { get; set; }

        public async Task<IList<Contract>> GetListAsync ()
        {
            await Task.CompletedTask;

            return _list;
        }
    }
}
