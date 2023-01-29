using MediatR;

namespace CtaLineaWebApi.Application.Commands.Calendars
{
    public class DeleteCalendarRequest
        : IRequest<bool>
    {
        public int CalendarId { get; set; }
    }
}
