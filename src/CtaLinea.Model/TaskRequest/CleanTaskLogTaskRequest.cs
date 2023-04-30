using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.TaskRequest
{
	public class CleanTaskLogTaskRequest
	{
		public int? DailyRetention { get; set; }
		public int? WeeklyRetention { get; set; }
		public int? MonthlyRetention { get; set; }
	}
}
