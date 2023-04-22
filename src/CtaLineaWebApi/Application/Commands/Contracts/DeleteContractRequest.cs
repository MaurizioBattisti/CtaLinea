using CtaLinea.Model;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Contracts
{
    public class DeleteContractRequest
        : IRequest<OperationResult<bool>>
    {
        public int ContractId { get; set; }
    }
}
