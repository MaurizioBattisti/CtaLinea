using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class AddSuspensionToRunsRequest
        : IRequest<MultiRunOperationResponse>
    {
        public MultiRunSetSuspensionRequest Data { get; set; }
    }
}
