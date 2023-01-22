using CtaLinea.Model.Helpers;
using CtaLinea.Model.ModelServices;
using CtaLinea.Model.Runs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Services
{
    public class CompleteRunCheckerService 
        : ICompleteRunCheckerService
    {
        private readonly ILogger _logger;
        private readonly IRunChecker _Checker;

        public CompleteRunCheckerService(
            IRunChecker Checker,
            ILogger<CompleteRunCheckerService> logger)
        {
            _Checker = Checker;
            _logger = logger;
        }
        public async Task<RunCheckResult> CheckRunAsync(RunItem run)
        {
            await this._Checker.CleanGraphAsync(run);
            var result = await _Checker.CheckRunAsync(run);

            // eseue anche i controli sul database


            return result;
        }
    }
}
