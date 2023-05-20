using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.ScheduledTasks;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Account
{
    public class UserService 
        : IUserService
    {
        private readonly IHttpService _http;

        public UserService(
            IHttpService htto)
        {
            _http = htto;
        }

        public async Task<UserQueryModel?> GetOneAsync(string userName)
        {
            var url = string.Format(Constants.Endpoint_Users_One_Fmt, userName);
            var data = await this._http.Get<UserQueryModel>(url);
            return data;
        }
        public async Task InsertAsync(NewUserModel model)
        {
            await this._http.Post<CheckResult>(Constants.Endpoint_Users, model);
        }
        public async Task UpdateAsync(string userName, EditUSerModel model)
        {
            var url = string.Format(Constants.Endpoint_Users_One_Fmt, userName);
            await this._http.Put<CheckResult>(url, model);
        }
        public async Task DeleteAsync(string userName)
        {
            var url = string.Format(Constants.Endpoint_Users_One_Fmt, userName);
            await this._http.Delete(url);
        }

        public async Task ResetPasswordASync(string userName, ResetUserPasswordModel model)
        {
            var url = string.Format(Constants.Endpoint_Users_ResetPwd_Fmt, userName);
            await this._http.Post<CheckResult>(url, model);
        }
    }
}
