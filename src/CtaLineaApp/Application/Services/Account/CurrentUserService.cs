using CtaLineaApp.Application.Model.Account;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Account
{
    public class CurrentUserService 
        : ICurrentUserService
    {
        private ILocalStorageService _localStorageService;

        public CurrentUserService(
            ILocalStorageService localStorageService
            )
        {
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<UserModel>(Constants.LoalStorageKey_User);
        }

        private UserModel? _User;
        public UserModel? User
        {
            get { return _User; }
            private set
            {
                _User = value;
                if (UserStatusChanged != null) this.UserStatusChanged();
            }
        }
        public Action? UserStatusChanged { get; set; } = null;

        public async Task SetUser(UserModel? user)
        {
            if (user == null)
            {
                await _localStorageService.RemoveItem(Constants.LoalStorageKey_User);
                this.User = null;
            }
            else
            {
                await _localStorageService.SetItem(Constants.LoalStorageKey_User, user);
                this.User = user;
            }
        }

    }
}
