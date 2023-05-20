using CtaLineaApp.Application.Model.Account;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Account
{
    public class AccountService
        : IAccountService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ICurrentUserService _currentUserSvc;

        public UserModel? User => _currentUserSvc.User;
        
        // public Action? UserStatusChanged { get => this._currentUserSvc.UserStatusChanged; set => this._currentUserSvc.UserStatusChanged = value; }
        public event EventHandler UserStatusChanged;

        protected virtual void OnThresholdReached(EventArgs e)
        {
            EventHandler handler = UserStatusChanged;
            handler?.Invoke(this, e);
        }

        public AccountService(
            IHttpService httpService,
            ICurrentUserService currentUserSvc,
            NavigationManager navigationManager
            )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _currentUserSvc = currentUserSvc;
            _currentUserSvc.UserStatusChanged = () => this.OnThresholdReached(new EventArgs());
        }

        public async Task Login(LoginModel model)
        {
            await this._currentUserSvc.SetUser(
                await _httpService.Post<UserModel, string>("api/auth/login", model)
                );
        }
        public async Task Logout()
        {
            await this._currentUserSvc.SetUser(null);
            _navigationManager.NavigateTo("/");
        }

        public async Task ChangePassword(ChangePasswordModel model)
        {
            await _httpService.Post<string>("api/auth/password", model);
        }

        public async Task Initialize()
        {
            await _currentUserSvc.Initialize();
        }

        public bool IsUserInRole (string role)
        {
            bool inRole = false;
            if (this.User != null)
            {
                inRole = this.User.Roles.Contains(role);
            }

            return inRole;
        }
        public bool IsUserInRoles(IEnumerable<string> roles)
        {
            bool inRole = false;
            if (this.User != null)
            {
                var count = this.User.Roles.Where(r => roles.Contains(r)).Count();
                inRole = count > 0;
            }

            return inRole;
        }
    }
}
