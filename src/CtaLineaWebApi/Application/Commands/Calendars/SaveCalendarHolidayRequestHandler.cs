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
    public class SaveCalendarHolidayRequestHandler
        : IRequestHandler<SaveCalendarHolidayRequest, OperationResult<CalendarHoliday>>
    {
        private readonly ILogger _logger;
        private readonly ICalendarRepository _repo;

        public SaveCalendarHolidayRequestHandler(
            ICalendarRepository repo,
            ILogger<SaveCalendarHolidayRequestHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<OperationResult<CalendarHoliday>> Handle(
            SaveCalendarHolidayRequest request, 
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<CalendarHoliday> () {  Success = true };

            try
            {
                if (request.Insert == true)
                {
                    result.Data = await this._repo.InsertHolidayAsync(request.Data);
                }
                else
                {
                    result.Data = await this._repo.UpdateHolidayAsync(request.Data);
                }
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
