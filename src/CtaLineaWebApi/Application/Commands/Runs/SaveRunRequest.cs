using CtaLinea.Model;
using CtaLinea.Model.Runs;
using MediatR;
using System;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class SaveRunRequest
        : IRequest<OperationResult<Guid>>
    {
        public Guid? RunId { get; set; }

        public RunItem? RunItem { get; set; }
    }
}
