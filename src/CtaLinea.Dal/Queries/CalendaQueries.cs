using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.QueryHelper;
using CtaLinea.Model.QueryModel;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class CalendaQueries 
        : ICalendaQueries
    {
        private const string CalendarsSql_Table = "dbo.vw_Calendars c";

        private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;

        public CalendaQueries(
            CtaDbContext context,
            IZzRequestConstx zzContext)
        {
            this._context = context;
            this._zzContext = zzContext;
        }

        // Calendari
        public async Task<QueryItemList<CalendarQueryItem>> GetCalendarListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<CalendarQueryItem>(
                CalendarsSql_Table,
                filterContext);

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<CalendarQueryItem> GetOneCalendarAsync(
            int id)
        {
            var queryDef = new QueryDefinition<CalendarQueryItem>(
                CalendarsSql_Table,
                null,
                "c.CalendarId = @CalendarId",
                new { CalendarId = id });

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
