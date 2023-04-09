using CtaLinea.Model.QueryModel;
using CtaLineaApp.Application.Model;
using CtaLineaApp.Shared;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace CtaLineaApp
{
    public class CtaLineaGlobalFiltersManager
        : ComponentBase
    {
        [Parameter]
        public EventCallback<CtaLineaGlobalFilters> Changed { get; set; }

        [Parameter]
        public EventCallback<ContractQueryItem?> ContractChanged { get; set; }
        [Parameter]
        public EventCallback<OperationPeriodQueryItem?> PeriodChanged { get; set; }

        [CascadingParameter]
        public CtaLineaGlobalFilters Filters { get; set; } = new CtaLineaGlobalFilters();

        public async Task SetContractAsync (ContractQueryItem? contract)
        {
            this.Filters.Contract = contract;
            await this.ContractChanged.InvokeAsync(this.Filters.Contract);
            await this.Changed.InvokeAsync(this.Filters);
        }
        public async Task SetPeriodAsync(OperationPeriodQueryItem? period)
        {
            this.Filters.Period  =period;
            await this.PeriodChanged.InvokeAsync(this.Filters.Period);
            await this.Changed.InvokeAsync(this.Filters);
        }
    }
}
