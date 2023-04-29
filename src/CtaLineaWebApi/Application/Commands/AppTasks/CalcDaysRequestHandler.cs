using CtaLineaWebApi.Application.Scheduler;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class CalcDaysRequestHandler
		: IRequestHandler<CalcDaysRequest, bool>
	{
		private readonly ILogger _logger;

		public CalcDaysRequestHandler(
			ILogger<CalcDaysRequestHandler> logger
			)
		{
			_logger = logger;
		}

		public async Task<bool> Handle(
			CalcDaysRequest request, 
			CancellationToken cancellationToken)
		{
			await Task.CompletedTask;



			return true;
		}
	}
}
