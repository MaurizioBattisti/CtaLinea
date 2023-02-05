using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Calendar
{
    public class CalendarHoliday
    {
        public int CalendarId { get; set; }
        public DateTime Holiday { get; set; }
        public string? HolidayDescription { get; set; }

        public void CopyFrom(CalendarHoliday source)
        {
            this.CalendarId = source.CalendarId;
            this.Holiday = source.Holiday;

            this.HolidayDescription = source.HolidayDescription;
        }

    }
}
