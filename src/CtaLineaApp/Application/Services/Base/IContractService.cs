using CtaLinea.Model.Base;
using CtaLinea.Model.QueryModel;

namespace CtaLineaApp.Application.Services.Base
{
    public interface IContractService
    {
        Contract? Current { get; set; }
        OperationPeriodQueryItem? CurrentPEriod { get; set; }
    }
}