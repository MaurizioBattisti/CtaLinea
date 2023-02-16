using CtaLinea.Model;
using CtaLinea.Model.Base;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Tags
{
    public class InsertTagRequest
        : IRequest<OperationResult<int>>
    {
        public TagForRun Model { get; set; }
    }
}
