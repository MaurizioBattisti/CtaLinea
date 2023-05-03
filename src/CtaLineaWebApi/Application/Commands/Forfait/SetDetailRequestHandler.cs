using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class SetDetailRequestHandler
		:IRequestHandler<SetDetailRequest, bool>
	{
		private readonly IForfaitRepository _repository;
		private readonly ILogger _logger;

		public SetDetailRequestHandler (
			IForfaitRepository repository,
			ILogger<SetDetailRequestHandler> logger
			)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task<bool> Handle(
			SetDetailRequest request, 
			CancellationToken cancellationToken)
		{
			if (request.ForfaitId == null
				&& (request.RunIds == null
					|| request.RunIds.Count() == 0
				))
			{
				return false;
			}

			if (request.ForfaitId != null
				&& (request.RunIds == null
					|| request.RunIds.Count() == 0
				))
			{
				await this._repository.RemoveAllRunsAsync(request.ForfaitId.Value)
					.ConfigureAwait(false);
			}
			else if (request.ForfaitId == null)
			{
				await this._repository.RemoveRunsAsync(request.RunIds)
					.ConfigureAwait(false);
			}
			else
			{
				await this._repository.AddRunsASync(
					request.ForfaitId.Value,
					request.RunIds)
					.ConfigureAwait(false);
			}

			return true;
		}
	}
}
