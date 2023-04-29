using CtaLineaWebApi.Application.Scheduler;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
	public class ReloadSChdulerRequestHandler
		: IRequestHandler<ReloadSChdulerRequest, bool>
	{
		private readonly ILogger _logger;
		private readonly ISchedulerService _scheduler;

		public ReloadSChdulerRequestHandler (
			ISchedulerService scheduler,
			ILogger<ReloadSChdulerRequestHandler> logger
			)
		{
			_scheduler = scheduler;
			_logger = logger;
		}

		public async Task<bool> Handle(
			ReloadSChdulerRequest request, 
			CancellationToken cancellationToken)
		{
			await this._scheduler.ReloadSchedulerAsync()
				.ConfigureAwait(false);

			return true;
		}
	}
}
