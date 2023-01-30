using CtaLinea.Model;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Calendars
{
    public class DeleteCalendarPeriodRequest
        : IRequest<OperationResult<bool>>
    {
        public int CalendarPeriodId { get; set; }
    }
}
