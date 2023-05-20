using CtaLinea.Model.Base;
using CtaLineaWebApi.Auth.Model;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model;

namespace CtaLineaWebApi.Auth.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model);
        Task<ChangePasswordResult> ChangePasswordASync(string userName, string oldPAssword, string newPAssowrd);

        Task<AuthenticateResponse> AuthentifateSystemUserAsync();

        Task InsertNewUSerAsync(
            NewUserModel model);
        Task UpdateUSerAsync(
            string userName,
            EditUSerModel model);
        Task DeleteUserAsync(
           string userName);

        Task SetUserPAsswordAsync(
            string userName,
            string newPAssowrd,
            bool setMustChange = true);
    }
}