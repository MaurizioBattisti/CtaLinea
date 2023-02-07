using CtaLinea.Model.Base;
using CtaLinea.Model.Calendar;
using CtaLinea.Model.Helpers;
using CtaLineaApp.Application.Services.Helper;
using System;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace CtaLineaApp.Application.Services.Base
{
    public class CalendarService 
        : ICalendarService
    {
        private readonly IHttpService _http;

        public CalendarService (
            IHttpService htto)
        {
            _http = htto;
        }

        public async Task<Calendar?> GetOneAsync(int calendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_SingleOne_Fmr, calendarId);
            var data = await _http.Get<Calendar?> (url);
            return data;
        }
        public async Task UpdateAsync (Calendar calendar)
        {
            var url = string.Format(Constants.Endpoint_Calendar_One_Fmr, calendar.CalendarId);
            await this._http.Put<CheckResult>(url, calendar);
        }
        public async Task<int> InsertAsync(Calendar calendar)
        {
            return await this._http.Post<int, CheckResult>(Constants.Endpoint_Calendars, calendar);
        }
        public async Task DeleteAsync (int calendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_One_Fmr, calendarId);
            await this._http.Delete(url);
        }

        public async Task<IEnumerable<CalendarPeriod>?> GetPeriodListASync(
            int calendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Periods_Fmr, calendarId);
            var data = await _http.Get<IEnumerable<CalendarPeriod>?>(url);
            return data;
        }
        public async Task<CalendarPeriod?> InsertPeriodASync(CalendarPeriod period)
        {
            return await this._http.Post<CalendarPeriod, CheckResult>(Constants.Endpoint_CalendarPeriods, period);
        }
        public async Task UpdatePeriodAsync(CalendarPeriod period)
        {
            var url = string.Format(Constants.Endpoint_CalendarPeriods_Single_Fmt, period.CalendarPeriodId);
            await this._http.Put<CheckResult>(url, period);
        }
        public async Task DeletePeriodAsync(int periodId)
        {
            var url = string.Format(Constants.Endpoint_CalendarPeriods_Single_Fmt, periodId);
            await this._http.Delete(url);
        }

        public async Task<IEnumerable<CalendarHoliday>?> GetHolidayListAsync(
            int calendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Holidays_Fmr, calendarId);
            var data = await _http.Get<IEnumerable<CalendarHoliday>?>(url);
            return data;
        }

        public async Task InsertHolidayASync(CalendarHoliday holiday)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Holidays_Fmr, holiday.CalendarId);
            await this._http.Post<CheckResult>(url, holiday);
        }
        public async Task UpdateHolidayAsync(CalendarHoliday holiday)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Holidays_Single_Fmr, holiday.CalendarId, holiday.Holiday);
            await this._http.Put<CheckResult>(url, holiday);
        }
        public async Task DeleteHolidayAsync(int calendarId, DateTime date)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Holidays_Single_Fmr, calendarId, date);
            await this._http.Delete(url);
        }

        public async Task<IEnumerable<SingleCalendarDay>?> GetDaysAsync (
            int calendarId,
            DateTime startDate,
            DateTime endDate)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Days,
                calendarId,
                startDate,
                endDate);
            var data = await _http.Get<IEnumerable<SingleCalendarDay>?>(url);
            return data;
        }
    }
}

