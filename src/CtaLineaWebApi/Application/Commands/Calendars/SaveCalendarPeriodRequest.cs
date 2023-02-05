using CtaLinea.Model;
using CtaLinea.Model.Calendar;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Calendars
{
    public class SaveCalendarPeriodRequest
        : IRequest<OperationResult<CalendarPeriod>>
    {
        public bool Insert { get; set; } = false;
        public CalendarPeriod Data { get; set; }
    }
}
