using CtaLineaApp.Application.Services.Account;
using Microsoft.AspNetCore.Components;

namespace CtaLineaApp.Components
{
    public class CheckAuthorization
        : ComponentBase
    {
        [Inject]
        public NavigationManager? _navigationManager { get; set; }
        [Inject]
        public IAccountService? _accountService { get; set; }

        [Parameter]
        public IEnumerable<string> Roles { get; set; } = new List<string>();

        protected override void OnInitialized()
        {
            var authorized = false;
            if (_accountService != null)
            {
                authorized = this._accountService.IsUserInRoles(this.Roles ?? new List<string>());
            }

            if (authorized == false)
            {
                _navigationManager?.NavigateTo("/forbidden");
            }
        }
    }
}
