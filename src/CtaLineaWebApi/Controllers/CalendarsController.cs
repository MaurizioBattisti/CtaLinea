using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Calendar;
using ZzSoft.CtaLinea.Dal.Repositories;
using CtaLinea.Model.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using CtaLineaWebApi.Application.Commands.Calendars;
using MediatR;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/calendars")]
    public class CalendarsController
        : ZControllerBase
    {
        private readonly ISender _mediator;
        private readonly ICalendaQueries _queries;
        private readonly ICalendarRepository _repo;
       
        public CalendarsController(
            ISender mediator,
            ICalendaQueries queries,
            ICalendarRepository repo)
        {
            _mediator = mediator;
            this._queries = queries;
            _repo = repo;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarQueryItem>>> GetAllAsync()
        {
            var result = await this._queries.GetCalendarListAsync(
                this.FilteringContext)
                .ConfigureAwait (false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CalendarQueryItem>> GetOneAsync(
            int id)
        {
            var result = await this._queries.GetOneCalendarAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [SwaggerOperation("restituisce i dati semplici di un calendario, qeulli che poi andranno inviati nelle operaizoni di modifica e inserimetno calendario")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/simple")]
        public async Task<ActionResult<Calendar>> GetSimpleCalendarAsync(
            int id)
        {
            var result = await this._repo.GetOneAsync(id)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
        [SwaggerOperation("Crea un nuovo calendario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> InsertAsync(
            [FromBody] Calendar model)
        {
            var request = new SaveCalendarRequest()
            {
                Insert = true,
                Data = model
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Inserimetno calendario Fallito",
                        Detail = result.Message
                    });
            }

            return this.Created(
                string.Format("/{0}", result.Data),
                null);
        }
        [SwaggerOperation("Aggiorna i dati di un calendario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] Calendar model)
        {
            model.CalendarId = id;

            var request = new SaveCalendarRequest()
            {
                Insert = false,
                Data = model
            };
            var result =await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Salvataggio calendario Fallito",
                        Detail = result.Message
                    });
            }

            return Ok();
        }

        [SwaggerOperation("Elimina un calendario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteASync(
            [FromRoute] int id)
        {
            var request = new DeleteCalendarRequest()
            {
                CalendarId = id
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);
            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Eliminaizone calendario fallita",
                        Detail = result.Message
                    });
            }

            return NoContent();
        }

        #region periodi dei calendari
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id:int}/periods")]
        public async Task<ActionResult<IEnumerable<CalendarPeriod>>> GetPeriodListAsync(
            [FromRoute] int id)
        {
            var result = await this._repo.GetPeriodListAsync(id)
                .ConfigureAwait(false);

            return this.Ok(result);
        }
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("periods/{id:int}")]
        public async Task<ActionResult<CalendarPeriod>> GetOnePeriodAsync(
            [FromRoute] int id)
        {
            var result = await this._repo.GetOnePeriodAsync(id)
                .ConfigureAwait(false);

            return this.Ok(result);
        }

        [SwaggerOperation("Crea un periodo di calendaraio")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        [Route("periods")]
        public async Task<IActionResult> InsertPeriodASync(
            [FromBody] CalendarPeriod model)
        {
            var request = new SaveCalendarPeriodRequest()
            {
                Insert = true,
                Data = model
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);
            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "inserimento periodo calendario Fallito",
                        Detail = result.Message
                    });
            }

            return this.Created(
                string.Format("/periods/{0}", result.Data),
                null);
        }
        [SwaggerOperation("Aggiorna i dati di un periodo di calendario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("periods/{id:int}")]
        public async Task<IActionResult> UpdatePeriodAsync(
            [FromRoute] int id,
            [FromBody] CalendarPeriod model)
        {
            model.CalendarId = id;

            var request = new SaveCalendarPeriodRequest()
            {
                Insert = false,
                Data = model
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "salvataggio periodo calendario Fallito",
                        Detail = result.Message
                    });
            }

            return Ok();
        }
        [SwaggerOperation("Elimina un periodo di calendario calendario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("periods/{id:int}")]
        public async Task<IActionResult> DeletePeriodASync(
            [FromRoute] int id)
        {
            var request = new DeleteCalendarPeriodRequest()
            {
                CalendarPeriodId = id
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "eliminaizone periodo calendario Fallita",
                        Detail = result.Message
                    });
            }

            return NoContent();
        }
        #endregion

        #region giorni dei calendari
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id:int}/holidays")]
        public async Task<ActionResult<IEnumerable<CalendarHoliday>>> GetHolidayListAsync(
            [FromRoute] int id)
        {
            var result = await this._repo.GetHolidaysAsync(id)
                .ConfigureAwait(false);

            return this.Ok(result);
        }

        [SwaggerOperation("Crea un giorno del calendaraio")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        [Route("{id:int}/holidays")]
        public async Task<IActionResult> InsertHolidaydASync(
            [FromRoute] int id,
            [FromBody] CalendarHoliday model)
        {
            model.CalendarId = id;

            var request = new SaveCalendarHolidayRequest()
            {
                Insert = true,
                Data = model
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "inserimento giorno calendario Fallito",
                        Detail = result.Message
                    });
            }

            return this.Created(
                string.Format("/{0}/holidays/{1:yyy-MM-dd}", id, model.Holiday),
                null);
        }
        [SwaggerOperation("Aggiorna i dati di un giorno  del calendario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("{id:int}/holidays/{date}")]
        public async Task<IActionResult> UpdateHolidaydASync(
            [FromRoute] int id,
            [FromRoute] DateTime date,
            [FromBody] CalendarHoliday model)
        {
            model.CalendarId = id;
            model.Holiday = date;

            var request = new SaveCalendarHolidayRequest()
            {
                Insert = false,
                Data = model
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);
            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Salvataggio giorno calendario Fallito",
                        Detail = result.Message
                    });
            }

            return Ok();
        }
        [SwaggerOperation("Elimina un giorno del calendario calendario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("{id:int}/holidays/{date}")]
        public async Task<IActionResult> DeleteHolidaydASync(
            [FromRoute] int id,
            [FromRoute] DateTime  date)
        {
            var request = new DeleteCalendarHolidayRequest()
            {
                CalendarId = id,
                Date = date
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            if (result.Success == false)
            {
                return Conflict(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Eliminaizone giorno calendario Fallita",
                        Detail = result.Message
                    });
            }

            return NoContent();
        }
        #endregion
    }
}
