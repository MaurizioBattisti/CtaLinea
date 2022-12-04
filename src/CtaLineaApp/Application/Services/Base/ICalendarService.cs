using CtaLinea.Model.Base;

namespace CtaLineaApp.Application.Services.Base
{
    public interface ICalendarService
    {
        Task<IList<Calendar>> GetCalendarListAsync();
    }
}