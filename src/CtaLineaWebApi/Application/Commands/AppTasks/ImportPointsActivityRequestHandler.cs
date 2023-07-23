using CtaLineaWebApi.Application.Commands.Budgets;
using CtaLineaWebApi.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class ImportPointsActivityRequestHandler
        : IRequestHandler<ImportPointsActivityRequest, bool>
    {
        private readonly ILogger _logger;
        private readonly ImporterConfiguration _options;

        public ImportPointsActivityRequestHandler(
            ImporterConfiguration options,
            ILogger<ImportPointsActivityRequestHandler> logger
            )
        {
            _options = options;
            _logger = logger;
        }

        public async Task<bool> Handle(
            ImportPointsActivityRequest request, 
            CancellationToken cancellationToken)
        {

            string se = this._options.PointsFilePath;

            await Task.CompletedTask;

            return true;
        }
    }
}
