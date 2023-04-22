using CtaLinea.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Contracts
{
    public class UpdateContractRequestHandler
        : IRequestHandler<UpdateContractRequest, OperationResult<bool>>
    {
        private readonly IContractRepository _repo;
        private readonly ILogger _logger;

        public UpdateContractRequestHandler(
			IContractRepository repo,
            ILogger<UpdateContractRequestHandler> logger
            )
        {
            _repo = repo;
            _logger= logger;
        }

        public async Task<OperationResult<bool>> Handle(
            UpdateContractRequest request, 
            CancellationToken cancellationToken)
        {
            await _repo.UpdateASync(request.Model);
            return new OperationResult<bool>()
            {
				Success = true,
				Data = true
            };
        }
    }
}
