using CtaLinea.Model.Runs;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class GetOneRunItemRequestHandler
        : IRequestHandler<GetOneRunItemRequest, RunItem?>
    {
        private readonly IRunRepository _runRepo;
        private readonly ILogger _logger;

        public GetOneRunItemRequestHandler (
            IRunRepository runRepo,
            ILogger<GetOneRunItemRequestHandler> logger)
        {
            _runRepo = runRepo;
            _logger = logger;
        }

        public async Task<RunItem?> Handle(
            GetOneRunItemRequest request, 
            CancellationToken cancellationToken)
        {
            return await _runRepo.GetOneRunItemAsync (
                request.RunId)
                .ConfigureAwait (false);
        }
    }
}
