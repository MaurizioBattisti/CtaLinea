using CtaLinea.Model;
using MediatR;
using System;

namespace CtaLineaWebApi.Application.Commands.Calendars
{
    public class DeleteCalendarHolidayRequest
        : IRequest<OperationResult<bool>>
    {
        public int CalendarId { get; set; }
        public DateTime Date { get; set; }
    }
}