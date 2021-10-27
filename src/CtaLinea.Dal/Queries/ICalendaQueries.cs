using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface ICalendaQueries
    {
        Task<QueryItemList<CalendarQueryItem>> GetCalendarListAsync(
            IFilteringContext filterContext);
        Task<CalendarQueryItem> GetOneCalendarAsync(
            string id);
    }
}