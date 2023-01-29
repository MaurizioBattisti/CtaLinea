using CtaLinea.Model.Base;
using CtaLinea.Model.Calendar;
using CtaLineaApp.Application.Services.Helper;
using System.Globalization;
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

        public async Task<IEnumerable<CalendarPeriod>?> GetPeriodListASync(
            int claendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Periods_Fmr, claendarId);
            var data = await _http.Get<IEnumerable<CalendarPeriod>?>(url);
            return data;
        }
        public async Task<IEnumerable<CalendarHoliday>?> GetHolidayListAsync(
            int claendarId)
        {
            var url = string.Format(Constants.Endpoint_Calendar_Holidays_Fmr, claendarId);
            var data = await _http.Get<IEnumerable<CalendarHoliday>?>(url);
            return data;
        }
    }
}

