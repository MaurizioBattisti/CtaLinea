using CtaLineaApp.Shared;
using Microsoft.AspNetCore.Components;

namespace CtaLineaApp
{
    public class LayoutSetter 
        : ComponentBase
    {
        [CascadingParameter]
        public MainLayout? Layout { get; set; }

        [Parameter]
        public RenderFragment? TopLine { get; set; }

        [Parameter]
        public RenderFragment? PageTitle { get; set; }

        protected override void OnInitialized()
        {
            Layout?.SetHeaderAndFooter(
                this.TopLine, 
                this.PageTitle);
        }
    }
}