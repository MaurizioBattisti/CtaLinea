using System.Threading.Tasks;
using CtaLinea.Model.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface ICalendaQueries
    {
        Task<QueryItemList<CalendarQueryItem>> GetCalendarListAsync(
            IFilteringContext filterContext);
        Task<CalendarQueryItem> GetOneCalendarAsync(
            int id);
    }
}