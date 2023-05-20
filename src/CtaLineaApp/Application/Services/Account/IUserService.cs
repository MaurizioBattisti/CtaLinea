using CtaLinea.Model.Base;
using CtaLinea.Model.QueryModel;

namespace CtaLineaApp.Application.Services.Account
{
    public interface IUserService
    {
        Task DeleteAsync(string userName);
        Task<UserQueryModel?> GetOneAsync(string userName);
        Task InsertAsync(NewUserModel model);
        Task ResetPasswordASync(string userName, ResetUserPasswordModel model);
        Task UpdateAsync(string userName, EditUSerModel model);
    }
}