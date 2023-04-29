using NPOI.OpenXmlFormats.Dml.Chart;
using System;

namespace CtaLineaWebApi.Application.Scheduler
{
	public class ScheduledTaskItem
	{
		public int Id { get; set; } 
		public string ActivityId { get; set; } = string.Empty;

		public ScheduleFrequency Frequency { get; set; } = ScheduleFrequency.Daily;
		public ushort RrequencyMask { get; set; } = 1;

		public TimeSpan StartTime { get; set; }= TimeSpan.FromHours(8);
		public TimeSpan EndTime { get; set; } = TimeSpan.FromHours(18);
		public TimeSpan Interval { get; set; }	= TimeSpan.FromMinutes(30);

		// in formato JSON
		public string Arguments { get; set; }
		public bool Active { get; set; } = true;

		public DateTime? LastStart { get; set; }
		public DateTime? LastEnd { get; set; }
	}
}
