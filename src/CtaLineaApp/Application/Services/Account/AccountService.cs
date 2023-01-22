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
        private ILocalStorageService _localStorageService;

        private UserModel? _User;
        public UserModel? User  
        {
            get {  return _User; }
            private set 
            { 
                _User = value;
                if (UserStatusChanged != null) this.UserStatusChanged();
            }
        }
        public Action? UserStatusChanged { get; set; } = null;

        public AccountService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
            )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<UserModel>(Constants.LoalStorageKey_User);
        }

        public async Task Login(LoginModel model)
        {
            this.User = await _httpService.Post<UserModel, string>("api/auth/login", model);
            await _localStorageService.SetItem(Constants.LoalStorageKey_User, this.User);
        }
        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem(Constants.LoalStorageKey_User);
            _navigationManager.NavigateTo("/");
        }

        public async Task ChangePassword(ChangePasswordModel model)
        {
            await Task.CompletedTask;
        }
    }
}
