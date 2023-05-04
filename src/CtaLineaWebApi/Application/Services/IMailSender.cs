using CtaLineaWebApi.Application.Model;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Services
{
    public interface IMailSender
    {
        Task SendEmailAsync(MailMessage message);
    }
}