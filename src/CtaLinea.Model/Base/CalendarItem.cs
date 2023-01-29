using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Base
{
    public class CalendarItem
    {
        public int CalendarId { get; set; }
        public int? BaseCalendarId { get; set; }
        public string CalendarName { get; set; } = String.Empty;
    }
}
