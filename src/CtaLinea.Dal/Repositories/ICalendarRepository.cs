using CtaLinea.Model.Calendar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface ICalendarRepository
    {
        Task DeleteAsync(int calendarId);
        Task DeleteHolidayAsync(int calendarId, DateTime date);
        Task DeletePeriodAsync(int calendarPeriodId);
        Task<IEnumerable<CalendarHoliday>> GetHolidaysAsync(int calendarId);
        Task<Calendar> GetOneAsync(int calendarId);
        Task<CalendarPeriod> GetOnePeriodAsync(int calendarPeriodId);
        Task<IEnumerable<CalendarPeriod>> GetPeriodListAsync(int calendarId);
        Task<Calendar> InsertAsync(Calendar model);
        Task<CalendarHoliday> InsertHolidayAsync(CalendarHoliday model);
        Task<CalendarPeriod> InsertPeriodASync(CalendarPeriod model);
        Task<Calendar> UpdateAsync(Calendar model);
        Task<CalendarHoliday> UpdateHolidayAsync(CalendarHoliday model);
        Task<CalendarPeriod> UpdatePeriodASync(CalendarPeriod model);

        Task<IEnumerable<SingleCalendarDay>> GetDaysAsync(
            int calendarId,
            DateTime startDate,
            DateTime endDate);
    }
}