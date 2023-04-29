using MediatR;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class CalcDaysRequest
		: IRequest<bool>
	{
		public int MaxRuns { get; set; }
	}
}
