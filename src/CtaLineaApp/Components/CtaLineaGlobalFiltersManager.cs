using CtaLinea.Model.QueryModel;
using CtaLineaApp.Application.Model;
using CtaLineaApp.Shared;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace CtaLineaApp
{
    public class CtaLineaGlobalFiltersManager
        : ComponentBase,
        IDisposable
    {
        [Parameter]
        public EventCallback<CtaLineaGlobalFilters> Changed { get; set; }

        [CascadingParameter]
        public CtaLineaGlobalFilters Filters { get; set; } = new CtaLineaGlobalFilters();

        protected override void OnInitialized()
        {
            Filters.FilterChange += this.FilterChanged;
        }

        public void Dispose()
        {
            Filters.FilterChange -= this.FilterChanged;
        }

        private void FilterChanged()
        {
            this.Changed.InvokeAsync();
        }
    }
}
