using CtaLinea.Model.Costs;
using CtaLinea.Model.Runs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class CostQueries 
        : ICostQueries
    {
        private const string SQL_up_CostsByAssociate = "[dbo].[up_GetCosts]";

        private readonly CtaDbContext _context;
        private readonly ILogger _logger;

        public CostQueries(
            CtaDbContext context,
            ILogger<CostQueries> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CostsByAssociate>> GetCostByAssociateAsync(
            int? contractId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            Guid? associateId = null,
            Guid? carId = null,
            Guid? runId = null
            )
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            using var reader = await conn.QueryMultipleAsync(
                SQL_up_CostsByAssociate,
                param: new
                {
                    ContractId = contractId,
                    StartDate = startDate,
                    EndDate = endDate,
                    AssociateId = associateId,
                    CarId = carId,
                    RunId = runId
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = reader.Read<CostsByAssociate>();
            return await Task.FromResult(items);
        }
    }
}
