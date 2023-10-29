using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Reports;
using CtaLinea.Model.Request;
using CtaLinea.Model.Utilities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.Services;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class ReportQueries 
        : IReportQueries
    {
        private readonly CtaDbContext _context;
        private readonly ICurrentUserService _userSvc;
        private readonly IZzRequestConstx _zzContext;

        public ReportQueries(
            ICurrentUserService userSvc,
            IZzRequestConstx zzContext,
            CtaDbContext context)
        {
            this._context = context;
            this._zzContext = zzContext;
            this._userSvc = userSvc;
        }
        public async Task<IEnumerable<NegativeKmItem>> GetNegativeKmAsync(
            NegativeKmReportRequest request)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);


            using var reader = await conn.QueryMultipleAsync(
                "[dbo].[up_Report_NegativeKms]",
                param: new
                {
                    ContractId = request.Contract,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    ConsiderSuspended = request.ConsiderSuspended
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            var items = reader.Read<NegativeKmItem>();
            return await Task.FromResult(items);
        }
    }
}
