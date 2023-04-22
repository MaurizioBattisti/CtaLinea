using CtaLinea.Model;
using CtaLinea.Model.Base;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Contracts
{
    public class UpdateContractRequest
        : IRequest<OperationResult<bool>>
    {
        public Contract Model { get; set; }
    }
}
