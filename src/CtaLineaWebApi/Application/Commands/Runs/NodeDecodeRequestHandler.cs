using Azure.Core;
using CtaLinea.Model.Runs;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.HSSF.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Services;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class NodeDecodeRequestHandler
        : IRequestHandler<NodeDecodeRequest, IEnumerable<RunNode>>
    {
        private readonly INodeMatchService _nodeService;
        private readonly ILogger _logger;

        public NodeDecodeRequestHandler(
            INodeMatchService nodeService,
            ILogger<NodeDecodeRequestHandler> logger)
        {
            _nodeService = nodeService;
            _logger = logger;
        }

        public async Task<IEnumerable<RunNode>> Handle(
            NodeDecodeRequest request, 
            CancellationToken cancellationToken)
        {
            IEnumerable<RunNode>? result = null;
            // tenta di processare la stringa
            var nodeToFind = this.IterateAndExtract(request.NodesText).ToList();

            if (nodeToFind != null
                && nodeToFind.Count > 0)
            {
                result = await this._nodeService.FindMatchingNodesAsync(
                    nodeToFind).ConfigureAwait(false);
            }

            return result ?? new List<RunNode>(); ;
		}

        private IEnumerable<Tuple<string, TimeSpan>> IterateAndExtract (
            string text)
        {
            bool validRow = false;
			foreach (var row in IterateRows (text))
            {
                if (validRow == false
                    && row.ToLower().Contains("cod. fermata"))
                {
                    validRow = true;
                    continue;
                }
                else if (validRow == false) continue;

				var rawCols = row.Split("\t");
                var cols = (from c in rawCols
                            where string.IsNullOrEmpty(c) == false
                            select c).ToArray();
                if (cols.Length < 3)
                {
                    validRow = false;
                    continue;
                }

                // recupera l'id della femrata
                string id = cols[0].Trim();
                // recupera l'orario
                if (TimeSpan.TryParse(cols[2].Replace(".", ":"), out TimeSpan hour) == true)
                {
                    if (hour.Days < 1)
                    { 
                        yield return new Tuple<string, TimeSpan>(id, hour);
                    }
                }
                else
                {
                    validRow = false;
                }
            }
        }

        private IEnumerable<string> IterateRows (
            string text)
        {
            string[] rows = text.Split("\n");
            foreach (var row in rows) 
            {
                if (string.IsNullOrWhiteSpace(row) == true) continue;

                yield return row;
            }
        }
    }
}
