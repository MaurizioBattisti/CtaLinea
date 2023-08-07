namespace CtaLineaApp.Application.Services.Helper
{
    public interface IGlobalFiltersService
    {
        int? ContractId { get;  }
        DateTime? PeriodStartDate { get;  }
        DateTime? PerioEndDate { get;  }

        void SetFilters(
            DateTime? startDate,
            DateTime? endDate,
            int? contractId);
    }
}