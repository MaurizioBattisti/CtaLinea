using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Context
{
    public static  class DbConnectionExtensions
    {
        public static async Task InitializeSession (
            this IDbConnection conn,
            IZzRequestConstx zzContext)
        {
            await conn.ExecuteAsync(
                "[dbo].[up_Session_SetInfo]",
                new
                {
                    PeriodStart = zzContext.PeriodStartDate,
                    PeriodEnd = zzContext.PeriodEndDate,
                    ContractId = zzContext.ContractId,
                    UserName = zzContext.UserName
                });
        }
    }
}
