using MediatR;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class CalcDaysRequest
		: BaseTaskRequest
	{
		public int MaxRuns { get; set; }
	}
}
