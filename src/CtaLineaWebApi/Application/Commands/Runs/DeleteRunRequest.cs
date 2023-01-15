using CtaLinea.Model;
using CtaLinea.Model.Runs;
using MediatR;
using System;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class DeleteRunRequest
        : IRequest<OperationResult<Guid>>
    {
        public Guid RunId { get; set; }
    }
}
