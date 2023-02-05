using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Calendar
{
    public class CalendarPeriod
    {
        public int CalendarPeriodId { get; set; }
        public int CalendarId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Note { get; set; }

        public void CopyFrom(CalendarPeriod source)
        {
            this.CalendarPeriodId = source.CalendarPeriodId;

            this.CalendarId = source.CalendarId;
            this.StartDate = source.StartDate;
            this.EndDate = source.EndDate;

            this.Note = source.Note;
        }
    }
}
