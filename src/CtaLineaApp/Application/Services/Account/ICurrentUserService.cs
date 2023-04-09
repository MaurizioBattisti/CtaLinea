using CtaLineaApp.Application.Model.Account;

namespace CtaLineaApp.Application.Services.Account
{
    public interface ICurrentUserService
    {
        UserModel? User { get; }
        Action? UserStatusChanged { get; set; }

        Task Initialize();
        Task SetUser(UserModel? user);
    }
}