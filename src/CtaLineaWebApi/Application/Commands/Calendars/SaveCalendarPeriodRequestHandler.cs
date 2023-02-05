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
    public class SaveCalendarPeriodRequestHandler
        : IRequestHandler<SaveCalendarPeriodRequest, OperationResult<CalendarPeriod>>
    {
        private readonly ILogger _logger;
        private readonly ICalendarRepository _repo;

        public SaveCalendarPeriodRequestHandler(
            ICalendarRepository repo,
            ILogger<SaveCalendarPeriodRequestHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<OperationResult<CalendarPeriod>> Handle(
            SaveCalendarPeriodRequest request, 
            CancellationToken cancellationToken)
        {
            CalendarPeriod? period = null;
            var result = new OperationResult<CalendarPeriod> () {  Success = true };

            try
            {
                if (request.Insert == true)
                {
                    period = await this._repo.InsertPeriodASync(request.Data);
                }
                else
                {
                    period = await this._repo.UpdatePeriodASync(request.Data);
                }
                result.Data = period;
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
