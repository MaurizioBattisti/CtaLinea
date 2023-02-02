using CtaLinea.Model.Base;
using CtaLinea.Model.Calendar;
using CtaLineaApp.Application.Services.Helper;
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

