using CtaLinea.Model;
using CtaLinea.Model.Calendar;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Calendars
{
    public class SaveCalendarHolidayRequest
        : IRequest<OperationResult<CalendarHoliday>>
    {
        public bool Insert { get; set; } = false;
        public CalendarHoliday Data { get; set; }
    }
}
