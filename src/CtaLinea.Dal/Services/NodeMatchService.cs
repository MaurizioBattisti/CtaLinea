using CtaLinea.Model.External;
using CtaLinea.Model.Runs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Services
{
    public class NodeMatchService 
        : INodeMatchService
    {
        private const string SQL_FindNode = "SELECT c.CollectionPointId, c.Description FROM [dbo].[CollectionPoints] c WHERE c.CollectionPointId IN ({0})";
        private const string SQç_ArgsPrefix = "@p_";

        private readonly CtaDbContext _context;
        private readonly ILogger _logger;

        public NodeMatchService(
            CtaDbContext context,
            ILogger<NodeMatchService> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RunNode>> FindMatchingNodesAsync(
            IEnumerable<Tuple<string, TimeSpan>> nodeCodes)
        {
            var result = (from n in nodeCodes
                          select new RunNode()
                          {
                              RunNodeId = Guid.NewGuid(),
                              ProgrNumber = 0,
                              CollectionPointId = n.Item1,
                              Hour = n.Item2
                          }).ToList();

            try
            {
                var ids = result.Select(x => x.CollectionPointId).Distinct();

                // crea la lista degli argomenti da cercare
                var sql = string.Format(SQL_FindNode, 
                    this.GetArgumentsNameListString(ids.Count(), SQç_ArgsPrefix));
                // crea l'oggetto cong li argomenti
                var args = this.GetArgumnesData(ids, SQç_ArgsPrefix);

                using IDbConnection conn = this._context.GetNewConnection();
                conn.Open();

                // esegue la query che restituisce i punti trovati
                var data = await conn.QueryAsync<CollectionPointSimple>(
                    sql, args);

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        var points = (from c in result
                                      where c.CollectionPointId == item.CollectionPointId
                                      select c);
                        foreach (var cp in points)
                        {
                            cp.CollectionPointData = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                throw;
            }

            return result;
        }

        private string GetArgumentsNameListString(
            int count,
            string argPrefix)
        {
			return string.Join(",", this.GetArgumentsNameList(count, argPrefix));
        }
        private IEnumerable<string> GetArgumentsNameList(
            int count,
            string argPrefix)
        {
            for (int index = 0; index < count; ++index)
            {
                yield return this.GetArgumentName(index, argPrefix);
            }
        }
        private string GetArgumentName(
            int index,
            string argPrefix)
        {
            return string.Format("{0}{1}", argPrefix, index);
        }
        private object GetArgumnesData(
            IEnumerable<object> ids,
            string argPrefix)
        {
            var args = new ExpandoObject();
            var argsDict = args as IDictionary<string, object>;
            int index = 0;
            foreach (var id in ids)
            {
                argsDict.Add(
                    this.GetArgumentName(index, argPrefix),
                    id);
                ++index;
            }

            return args;
        }
    }
}
