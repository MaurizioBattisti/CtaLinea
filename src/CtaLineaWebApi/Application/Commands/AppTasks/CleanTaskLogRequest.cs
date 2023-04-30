using MediatR;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class CleanTaskLogRequest
		: BaseTaskRequest
	{
		public int? DailyRetention { get; set; }
		public int? WeeklyRetention { get; set; }
		public int ? MonthlyRetention { get; set; }
	}
}
