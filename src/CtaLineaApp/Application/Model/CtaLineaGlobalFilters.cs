using CtaLinea.Model.QueryModel;
using Microsoft.EntityFrameworkCore.Storage;

namespace CtaLineaApp.Application.Model
{
    public class CtaLineaGlobalFilters
    {
        public event Action? FilterChange;

        private int? _old_ContractId;
        private DateTime? _old_PeriodStart;
        private DateTime? _old_PeriodEnd;

        public ContractQueryItem? Contract { get; set; }

        public OperationPeriodQueryItem? Period { get; set; }

        public bool IsFilterSet()
        {
            return (this.Contract?.Id != null
                && this.Period?.StartDate != null
                && this.Period?.EndDate != null);
        }

        public void RaiseChanged ()
        {
            // se i dati sono uguali non solleva nessuna modifcia
            if (Contract?.Id == _old_ContractId
                && Period?.StartDate == _old_PeriodStart
                && Period?.EndDate == _old_PeriodEnd
                )
            {
                return;
            }
            _old_ContractId = this.Contract?.Id;
            _old_PeriodStart = this.Period?.StartDate;
            _old_PeriodEnd = this.Period?.EndDate;

            this.FilterChange?.Invoke();
        }
    }
}
