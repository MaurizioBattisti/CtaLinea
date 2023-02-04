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
            await this._http.Put(url, calendar);
        }
        public async Task<int> InsertAsync(Calendar calendar)
        {
            return await this._http.Post<int, CheckResult>(Constants.Endpoint_Calendars, calendar);
        }

        public async Task<IEnumerable<CalendarPeriod>?> GetPeriodListASync(
            int calendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Periods_Fmr, calendarId);
            var data = await _http.Get<IEnumerable<CalendarPeriod>?>(url);
            return data;
        }
        public async Task<IEnumerable<CalendarHoliday>?> GetHolidayListAsync(
            int calendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Holidays_Fmr, calendarId);
            var data = await _http.Get<IEnumerable<CalendarHoliday>?>(url);
            return data;
        }
    }
}

