using CEC.Routing.Components;
using CEC.Routing.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;

namespace CtaLineaApp.Components
{
    public class EditorComponentBase 
        : ComponentBase
        , IRecordRoutingComponent
        , IDisposable
    {
        [Inject]
        public DialogService DialogService { get; set; }

        /// <summary>
        /// Injected Navigation Manager
        /// </summary>
        [Inject]
        public NavigationManager NavManager { get; set; }

        /// <summary>
        /// Injected User Session Object
        /// </summary>
        [Inject]
        public RouterSessionService RouterSessionService { get; set; }

        /// <summary>
        /// IRecordRoutingComponent implementation
        /// </summary>
        public string RouteUrl { get; set; }

        /// <summary>
        /// IRecordRoutingComponent implementation
        /// </summary>
        public bool IsClean { get; set; } = true;

        /// <summary>
        /// Boolean property set when the user attempts to exit a dirty component
        /// </summary>
        protected bool ExitAttempt { get; set; }

        /// <summary>
        /// Form Edit Context
        /// </summary>
        public EditContext EditContext { get; set; }

        protected override Task OnInitializedAsync()
        {
            this.RouteUrl = this.NavManager.Uri;
            this.RouterSessionService.ActiveComponent = this;
            this.RouterSessionService.NavigationCancelled += OnNavigationCancelled;
            return base.OnInitializedAsync();
        }

        /// <summary>
        /// Event Handler for the Navigation Cancelled event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void OnNavigationCancelled(object sender, EventArgs e)
        {
            this.ExitAttempt = true;
            var result = await  DialogService.Confirm(
                "Ci sono dati non salvati nella pagina. Desideri comunque abbandonarla ?",
                "Dati non Salvati",
                new ConfirmOptions() { OkButtonText = "Si, abbandona !", CancelButtonText= "No, rimani qui !" }
                );
            this.StateHasChanged();

            if (result == true)
            {
                this.IsClean = true;
                this.ExitAttempt = false;
                this.NavManager.NavigateTo(this.RouterSessionService.NavigationCancelledUrl);
            }
        }

        public void Dispose()
        {
            this.RouterSessionService.NavigationCancelled -= OnNavigationCancelled;
        }
    }
}