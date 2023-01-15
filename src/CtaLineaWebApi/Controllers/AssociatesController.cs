using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;
using CtaLinea.QueryModel;
using CtaLinea.Model.QueryModel;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/associates")]
    public class AssociatesController
        : ZControllerBase
    {
        private readonly IAssociatesQueries _queries;
       
        public AssociatesController(
            IAssociatesQueries queries)
        {
            this._queries = queries;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssociateQueryItem>>> GetAllAsync()
        {
            var result = await this._queries.GetAssociateListAsync(
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
        public async Task<ActionResult<AssociateQueryItem>> GetOneAsync(
            Guid id)
        {
            var result = await this._queries.GetOneAssociateAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/cars")]
        public async Task<ActionResult<CarQueryItem>> GetCarsAsync(
            Guid id)
        {
            var result = await this._queries.GetAssociateCarListAsync(
                this.FilteringContext,
                id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/drivers")]
        public async Task<ActionResult<DriverQueryItem>> GetDriversAsync(
            Guid id)
        {
            var result = await this._queries.GetAssociateDriverListAsync(
                this.FilteringContext,
                id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
    }
}
