using MediatR;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class BaseTaskRequest
		: IRequest<bool>
	{
		public int Timeout { get; set; } = 100;
	}
}
