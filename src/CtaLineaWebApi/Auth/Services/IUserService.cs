using CtaLineaWebApi.Auth.Model;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Auth.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse?> AuthenticateAsync(AuthenticateRequest model);
        Task<ChangePasswordResult> ChangePAsswordASync(string userName, string oldPAssword, string newPAssowrd);
    }
}