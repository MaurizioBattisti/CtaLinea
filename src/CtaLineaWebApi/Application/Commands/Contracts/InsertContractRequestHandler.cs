using CtaLinea.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Contracts
{
    public class InsertContractRequestHandler
		: IRequestHandler<InsertContractRequest, OperationResult<int>>
    {
        private readonly IContractRepository _repo;
        private readonly ILogger _logger;

        public InsertContractRequestHandler(
			IContractRepository repo,
            ILogger<InsertContractRequestHandler> logger
            )
        {
            _repo = repo;
            _logger= logger;
        }

        public async Task<OperationResult<int>> Handle(
			InsertContractRequest request, 
            CancellationToken cancellationToken)
        {
            var id = await _repo.InsertASync(request.Model);
            return new OperationResult<int>()
            {
				Success = true,
				Data = id,
            };
        }
    }
}
