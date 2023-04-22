using CtaLinea.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Contracts 
{
    public class DeleteContractRequestHandler
        : IRequestHandler<DeleteContractRequest, OperationResult<bool>>
    {
        private readonly IContractRepository _repo;
        private readonly ILogger _logger;

        public DeleteContractRequestHandler(
			IContractRepository repo,
            ILogger<DeleteContractRequestHandler> logger
            )
        {
            _repo = repo;
            _logger= logger;
        }

        public async Task<OperationResult<bool>> Handle(
            DeleteContractRequest request, 
            CancellationToken cancellationToken)
        {
            await _repo.DeleteASync(request.ContractId);
            return new OperationResult<bool>()
            {
                Success = true,
                Data = true
            };
        }
    }
}
