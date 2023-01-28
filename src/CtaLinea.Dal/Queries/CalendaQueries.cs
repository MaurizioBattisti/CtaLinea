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

        private CtaDbContext _context;

        public CalendaQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        // Calendari
        public async Task<QueryItemList<CalendarQueryItem>> GetCalendarListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<CalendarQueryItem>(
                CalendarsSql_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
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

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
