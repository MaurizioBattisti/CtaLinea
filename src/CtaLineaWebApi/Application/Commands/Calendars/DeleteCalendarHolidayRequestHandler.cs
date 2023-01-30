using CtaLinea.Model;
using CtaLinea.Model.Calendar;
using CtaLineaWebApi.Application.Commands.Runs;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Calendars
{
    public class DeleteCalendarHolidayRequestHandler
        : IRequestHandler<DeleteCalendarHolidayRequest, OperationResult<bool>>
    {
        private readonly ILogger _logger;
        private readonly ICalendarRepository _repo;

        public DeleteCalendarHolidayRequestHandler(
            ICalendarRepository repo,
            ILogger<DeleteCalendarHolidayRequestHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<OperationResult<bool>> Handle(
            DeleteCalendarHolidayRequest request, 
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<bool> () {  Success = true };

            try
            {
                await this._repo.DeleteHolidayAsync(request.CalendarId, request.Date);
                result.Data = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }
            return result;
        }
    }
}
