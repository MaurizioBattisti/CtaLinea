using CtaLineaApp.Application.Model.Account;

namespace CtaLineaApp.Application.Services.Account
{
    public interface IAccountService
    {
        UserModel? User { get; }
        Task Initialize();
        Task Login(LoginModel model);
        Task Logout();
        Task ChangePassword(ChangePasswordModel model);
        /*
        Task<IList<UserModel>> GetAll();
        Task<UserModel> GetById(string id);
        Task Update(string id, EditUser model);
        Task Delete(string id);
        */
    }
}
