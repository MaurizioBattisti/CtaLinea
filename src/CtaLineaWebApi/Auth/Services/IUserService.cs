using CtaLineaWebApi.Auth.Model;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Auth.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model);
        Task<ChangePasswordResult> ChangePasswordASync(string userName, string oldPAssword, string newPAssowrd);

        Task<AuthenticateResponse> AuthentifateSystemUserAsync();
	}
}