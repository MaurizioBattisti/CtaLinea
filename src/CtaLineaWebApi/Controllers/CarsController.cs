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
using CtaLinea.Model.Base;
using NPOI.SS.Formula.Functions;

namespace CtaLineaWebApi.Controllers
{
	[Authorize]
	[ApiController]
    [Route("api/cars")]
    public class CarsController
        : ZControllerBase
    {
        private readonly IAssociatesQueries _queries;
       
        public CarsController (
            IAssociatesQueries queries)
        {
            this._queries = queries;
        }

		[Authorize(Policy = Constants.Policy_ViewData)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarQueryItem>>> GetAllAsync()
        {
            var result = await this._queries.GetCarListAsync(
                this.FilteringContext)
                .ConfigureAwait (false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
		[Authorize(Policy = Constants.Policy_ViewData)]
		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<CarQueryItem>> GetOneAsync(
            Guid id)
        {
            var result = await this._queries.GetOneCarAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
    }
}
