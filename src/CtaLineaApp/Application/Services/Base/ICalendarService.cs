using CtaLinea.Model.Base;
using CtaLinea.Model.Calendar;

namespace CtaLineaApp.Application.Services.Base
{
    public interface ICalendarService
    {
        Task<IEnumerable<CalendarPeriod>?> GetPeriodListASync(
            int claendarId);
        Task<IEnumerable<CalendarHoliday>?> GetHolidayListAsync(
            int claendarId);
    }
}