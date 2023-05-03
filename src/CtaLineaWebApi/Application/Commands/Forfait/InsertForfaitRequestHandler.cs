using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class InsertForfaitRequestHandler
		: IRequestHandler<InsertForfaitRequest, int>
	{
		private readonly IForfaitRepository _repository;
		private readonly ILogger _logger;

		public InsertForfaitRequestHandler(
			IForfaitRepository repository,
			ILogger<InsertForfaitRequestHandler> logger
			)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task<int> Handle(
			InsertForfaitRequest request, 
			CancellationToken cancellationToken)
		{
			int id = await _repository.InsertASync(
				request.Model)
				.ConfigureAwait( false );

			return id;
		}
	}
}
