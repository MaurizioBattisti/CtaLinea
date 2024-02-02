using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using ZzSoft.CtaLinea.Dal.Repositories;
using MediatR;
using System.Linq;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class SetRunLockedRequestHandler
        : IRequestHandler<SetRunLockedRequest, bool>
    {
        private readonly IRunRepository _runRepo;
        private readonly ILogger _logger;

        public SetRunLockedRequestHandler(
            IRunRepository runRepo,
            ILogger<SetRunLockedRequestHandler> logger
            )
        {
            _runRepo = runRepo;
            _logger = logger;
        }

        public async Task<bool> Handle(
            SetRunLockedRequest request,
            CancellationToken cancellationToken)
        {
            if (request.RunIds == null
                    || request.RunIds.Count() == 0
                )
            {
                return false;
            }

            // imposta il blocco o lo toglie se la data è nulla
            await this._runRepo.SetRunsLockAsync(
                request.RunIds,
                request.Date, request.Note)
                .ConfigureAwait(false);

            return true;
        }
    }
}
