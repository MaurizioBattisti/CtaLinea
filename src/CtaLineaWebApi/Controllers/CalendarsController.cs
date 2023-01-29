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
        public async Task<ActionResult<Calendar>> InsertAsync(
            [FromBody] Calendar model)
        {
            var request = new SaveCalendarRequest()
            {
                Insert = true,
                Data = model
            };
            var result = await this._mediator.Send(request)
                .ConfigureAwait(false);

            return this.Created(
                string.Format("/{0}", result),
                null);
        }
        [SwaggerOperation("Aggiorna i dati di un calendario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Calendar>> UpdateAsync(
            [FromRoute] int id,
            [FromBody] Calendar model)
        {
            model.CalendarId = id;

            var request = new SaveCalendarRequest()
            {
                Insert = false,
                Data = model
            };
            await this._mediator.Send(request)
                .ConfigureAwait(false);

            return Ok();
        }

        [SwaggerOperation("Elimina un calendario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(RunCheckResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<Calendar>> UpdateAsync(
            [FromRoute] int id)
        {
            var request = new DeleteCalendarRequest()
            {
                CalendarId = id
            };
            await this._mediator.Send(request)
                .ConfigureAwait(false);

            return Ok();
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
        #endregion
    }
}
