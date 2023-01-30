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
    public class SaveCalendarRequestHandler
        : IRequestHandler<SaveCalendarRequest, OperationResult<int>>
    {
        private readonly ILogger _logger;
        private readonly ICalendarRepository _repo;

        public SaveCalendarRequestHandler (
            ICalendarRepository repo,
            ILogger<SaveCalendarRequestHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<OperationResult<int>> Handle(
            SaveCalendarRequest request, 
            CancellationToken cancellationToken)
        {
            Calendar? calendar = null;
            var result = new OperationResult<int> () {  Success = true };

            try
            {
                if (request.Insert == true)
                {
                    calendar = await this._repo.InsertAsync(request.Data);
                }
                else
                {
                    calendar = await this._repo.UpdateAsync(request.Data);
                }
                result.Data = calendar.CalendarId;
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
