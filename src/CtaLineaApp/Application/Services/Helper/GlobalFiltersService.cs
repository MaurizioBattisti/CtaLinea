namespace CtaLineaApp.Application.Services.Helper
{
    public class GlobalFiltersService 
        : IGlobalFiltersService
    {
        public DateTime? PeriodStartDate { get; private set; }
        public DateTime? PerioEndDate { get; private set; }
        public int? ContractId { get; private set; }

        public void SetFilters (
            DateTime? startDate,
            DateTime? endDate,
            int? contractId)
        {
            this.PeriodStartDate = startDate;
            this.PerioEndDate = endDate;
            this.ContractId = contractId;
        }
    }
}
