using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Services
{
    public interface INodeMatchService
    {
        Task<IEnumerable<RunNode>> FindMatchingNodesAsync(IEnumerable<Tuple<string, TimeSpan>> nodeCodes);
    }
}