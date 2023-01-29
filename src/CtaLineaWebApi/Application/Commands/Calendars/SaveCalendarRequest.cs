using CtaLinea.Model.Calendar;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Calendars
{
    public class SaveCalendarRequest
        : IRequest<int>
    {
        public bool Insert { get; set; } = false;
        public Calendar Data { get; set; }
    }
}
