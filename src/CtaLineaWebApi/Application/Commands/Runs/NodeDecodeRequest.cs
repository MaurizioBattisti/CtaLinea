using CtaLinea.Model;
using CtaLinea.Model.Runs;
using MediatR;
using System.Collections;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class NodeDecodeRequest
        : IRequest<IEnumerable<RunNode>>
    {
        public string NodesText { get; set; }
    }
}
