using CtaLinea.Model.QueryModel;

namespace CtaLineaApp.Application.Model
{
    public class CtaLineaGlobalFilters
    {
        public ContractQueryItem? Contract { get; set; }

        public OperationPeriodQueryItem? Period { get; set; }
    }
}
