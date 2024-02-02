using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.ScheduledTasks
{
    public class ScheduledTaskItem
    {
        public int Id { get; set; }
        public string ActivityId { get; set; } = string.Empty;
		public string? Description { get; set; }

		public ScheduleFrequency Frequency { get; set; } = ScheduleFrequency.Daily;
        public int RrequencyMask { get; set; } = 1;

        public TimeSpan StartTime { get; set; } = TimeSpan.FromHours(8);
        public TimeSpan EndTime { get; set; } = TimeSpan.FromHours(18);
        public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(30);

        // in formato JSON
        public string? Arguments { get; set; }
        public bool Active { get; set; } = true;

        public DateTime? LastStart { get; set; }
        public DateTime? LastEnd { get; set; }

        public int Timeout { get; set; } = 0;

        public string GetFrequencyDescription ()
        {
            var descr = string.Empty;
            switch (this.Frequency)
            {
                case ScheduleFrequency.Daily:
                    descr = "Quotidiana";
                    break;
                case ScheduleFrequency.Weekly:
                    descr = "Settimanale";
                    break;
                    case ScheduleFrequency.Monthly:
                    descr = "Mensile";
                    break;
            }
            return descr;
        }
        public string GetFrequencyMaskDescription ()
        {
            var descr = string.Empty;
            switch (this.Frequency)
            {
                case ScheduleFrequency.Weekly:
					descr = string.Join(", ", this.EnumerateWeeklyFrequency());
					break;
                case ScheduleFrequency.Monthly:
					descr = string.Format("Ogni {0} dek mese", this.RrequencyMask);
                    break;
            }
            return descr;
        }

        private IEnumerable<string> EnumerateWeeklyFrequency ()
        {
            if (this.Frequency == ScheduleFrequency.Weekly )
            {
                if ((this.RrequencyMask & 1) == 1) yield return "Lun";
                if ((this.RrequencyMask & 2) == 2) yield return "Mar";
                if ((this.RrequencyMask & 4) == 4) yield return "Mer";
                if ((this.RrequencyMask & 8) == 8) yield return "Gio";
                if ((this.RrequencyMask & 16) == 16) yield return "Ven";
                if ((this.RrequencyMask & 32) == 32) yield return "Sab";
                if ((this.RrequencyMask & 64) == 64) yield return "Dom";
            }
        }

        public void CopyFrom (ScheduledTaskItem item)
        {
            this.Id = item.Id;
            this.ActivityId = item.ActivityId;
            this.Description = item.Description;

			this.Frequency = item.Frequency;
            this.RrequencyMask = item.RrequencyMask;

            this.StartTime = item.StartTime;
            this.EndTime = item.EndTime;
            this.Interval = item.Interval;

            this.Arguments = item.Arguments;
            this.Active = item.Active;

            this.LastStart = item.LastStart;
            this.LastEnd = item.LastEnd;

            this.Timeout = item.Timeout;
        }
    }
}
