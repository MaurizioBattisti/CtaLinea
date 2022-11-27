using CtaLinea.Model.Base;

namespace CtaLineaApp.Application.Services.Base
{
    public interface IContractService
    {
        Contract? Current { get; set; }
    }
}