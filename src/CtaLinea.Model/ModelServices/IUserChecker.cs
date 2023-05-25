using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;

namespace CtaLinea.Model.ModelServices
{
    public interface IUserChecker
    {
        Task<CheckResult> CheckAsync(EditUSerModel user);
        Task<CheckResult> CheckAsync(NewUserModel user);
    }
}