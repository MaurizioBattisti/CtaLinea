using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class DeleteForfaitReuqestHandler
		: IRequestHandler<DeleteForfaitReuqest, bool>
	{
		private readonly IForfaitRepository _repository;
		private readonly ILogger _logger;

		public DeleteForfaitReuqestHandler(
			IForfaitRepository repository,
			ILogger<DeleteForfaitReuqestHandler> logger
			)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task<bool> Handle(
			DeleteForfaitReuqest request, 
			CancellationToken cancellationToken)
		{
			await this._repository.DeleteASync(
				request.Id)
				.ConfigureAwait( false );

			return true;
		}
	}
}
