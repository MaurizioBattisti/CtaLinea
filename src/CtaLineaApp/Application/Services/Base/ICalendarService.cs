using CtaLinea.Model.Base;
using CtaLinea.Model.Calendar;
using CtaLinea.Model.Helpers;

namespace CtaLineaApp.Application.Services.Base
{
    public interface ICalendarService
    {
        Task<Calendar?> GetOneAsync(int calendarId);

        Task UpdateAsync(Calendar calendar);
        Task<int> InsertAsync(Calendar calendar);
        Task DeleteAsync(int calendarId);

        Task<IEnumerable<CalendarPeriod>?> GetPeriodListASync(
            int calendarId);
        Task<CalendarPeriod?> InsertPeriodASync(CalendarPeriod period);
        Task UpdatePeriodAsync(CalendarPeriod period);
        Task DeletePeriodAsync(int periodId);

        Task<IEnumerable<CalendarHoliday>?> GetHolidayListAsync(
            int calendarId);
        Task InsertHolidayASync(CalendarHoliday holiday);
        Task UpdateHolidayAsync(CalendarHoliday holiday);
        Task DeleteHolidayAsync(int calendarId, DateTime date);

        Task<IEnumerable<SingleCalendarDay>?> GetDaysAsync(
            int calendarId,
            DateTime startDate,
            DateTime endDate);
    }
}