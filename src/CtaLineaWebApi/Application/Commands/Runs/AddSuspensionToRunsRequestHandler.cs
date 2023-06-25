using CtaLinea.Model.Response;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class AddSuspensionToRunsRequestHandler
        : IRequestHandler<AddSuspensionToRunsRequest, MultiRunOperationResponse>
    {
        private readonly IRunRepository _repo;
        public AddSuspensionToRunsRequestHandler (
            IRunRepository repo)
        {
            _repo = repo;
        }

        public async Task<MultiRunOperationResponse> Handle(
            AddSuspensionToRunsRequest request, 
            CancellationToken cancellationToken)
        {
            var opResults =await _repo.MultiRunAddSuspensionsAsync (
                request.Data.RunIds,
                request.Data.Arguments
                )
                .ConfigureAwait ( false );

            var success = (opResults.Values
                .Where(r => r.Success == false)
                .Any() == false);

            var result = new MultiRunOperationResponse()
            {
                Detail = opResults,
                Success = success,
                Message = success ? string.Empty : "Alcuni problemi nell'assegnare la siospensione alle corse"
            };

            return result;
        }
    }
}
