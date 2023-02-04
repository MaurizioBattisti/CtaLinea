using CtaLinea.Model.Base;
using CtaLinea.Model.Calendar;

namespace CtaLineaApp.Application.Services.Base
{
    public interface ICalendarService
    {
        Task<Calendar?> GetOneAsync(int calendarId);

        Task UpdateAsync(Calendar calendar);
        Task<int> InsertAsync(Calendar calendar);


        Task<IEnumerable<CalendarPeriod>?> GetPeriodListASync(
            int calendarId);
        Task<IEnumerable<CalendarHoliday>?> GetHolidayListAsync(
            int calendarId);
    }
}