using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class UpdateForfaitRequestHandler
		: IRequestHandler<UpdateForfaitRequest, bool>
	{
		private readonly IForfaitRepository _repository;
		private readonly ILogger _logger;

		public UpdateForfaitRequestHandler(
			IForfaitRepository repository,
			ILogger<UpdateForfaitRequestHandler> logger
			)
		{
			_repository = repository;
			_logger = logger;
		}

		public async Task<bool> Handle(
			UpdateForfaitRequest request, 
			CancellationToken cancellationToken)
		{
			request.Model.ForfaitId = request.Id;

			await this._repository.UpdateASync(
				request.Model)
				.ConfigureAwait ( false );	

			return true;
		}
	}
}
