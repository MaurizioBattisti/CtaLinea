using CtaLinea.Model;
using CtaLinea.Model.Base;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Contracts
{
    public class InsertContractRequest
		: IRequest<OperationResult<int>>
    {
        public Contract Model { get; set; }
    }
}
