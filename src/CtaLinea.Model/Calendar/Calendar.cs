using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Calendar
{
    public class Calendar
    {
        private const string CalendarType_NoOperation = "NOP";
        private const string CalendarType_Exclusion = "EXC";
        private const string CalendarType_Inclusion = "INC";
        private const string CalendarType_InvertPreviousLevel = "IPL";

        public int CalendarId { get; set; }
        public int? BaseCalendarId { get; set; }

        public string CalendarName { get; set; } = string.Empty;
        public string CalendarType { get; set; } = CalendarType_Exclusion;

        public RunCalendarType RunCalendarType
        {
            get 
            {
                RunCalendarType value = RunCalendarType.Exclusion;

                switch (this.CalendarType)
                {
                    case CalendarType_NoOperation:
                        value = RunCalendarType.NoOperation;
                        break;
                    case CalendarType_Exclusion:
                        value = RunCalendarType.Exclusion;
                        break;
                    case CalendarType_Inclusion:
                        value = RunCalendarType.Inclusion;
                        break;
                    case CalendarType_InvertPreviousLevel:
                        value = RunCalendarType.InvertPreviousLevel;
                        break;
                    default:
                        value = RunCalendarType.Undefined;
                        break;
                }
                return value;
            }
            set
            {
                switch (value)
                {
                    case RunCalendarType.NoOperation:
                        this.CalendarType = CalendarType_NoOperation;
                        break;
                    case RunCalendarType.Exclusion:
                        this.CalendarType = CalendarType_Exclusion;
                        break;
                    case RunCalendarType.Inclusion:
                        this.CalendarType = CalendarType_Inclusion;
                        break;
                    case RunCalendarType.InvertPreviousLevel:
                        this.CalendarType = CalendarType_InvertPreviousLevel;
                        break;
                    default:
                        this.CalendarType = CalendarType_Exclusion;
                        break;
                }
            }
        }

        public bool Sundays { get; set; }
        public bool PreHolyday { get; set; }
        public bool PostHolyday { get; set; }

        public void CopyFrom (Calendar source)
        {
            this.CalendarId = source.CalendarId;

            this.BaseCalendarId = source.BaseCalendarId;
            this.CalendarName = source.CalendarName;
            this.CalendarType = source.CalendarType;

            this.Sundays = source.Sundays;
            this.PreHolyday = source.PreHolyday;
            this.PostHolyday = source.PostHolyday;
        }
    }

    public enum RunCalendarType
    {
        NoOperation,
        Exclusion,
        Inclusion,
        InvertPreviousLevel,

        Undefined
    }
}
