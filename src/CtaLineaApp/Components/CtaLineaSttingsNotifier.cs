using CtaLineaApp.Application.Model;
using CtaLineaApp.Shared;
using Microsoft.AspNetCore.Components;

namespace CtaLineaApp
{
    public class CtaLineaSttingsNotifier
        : ComponentBase
    {
        [Parameter]
        public EventCallback<CtaLineaSettings> Changed { get; set; }

        [CascadingParameter]
        public CtaLineaSettings? Settings { get; set; } = null;

        public async Task IncresePageSizeAsync ()
        {
            if (Settings != null)
            {
                this.Settings.PageSize = this.Settings.PageSize + 1;
                await this.Changed.InvokeAsync(Settings);
            }
        }
    }
}
