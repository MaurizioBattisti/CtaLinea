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

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/collectionpoints")]
    public class CollectionPointsController
        : ZControllerBase
    {
        private readonly ICollectionPointsQueries _queries;
       
        public CollectionPointsController(
            ICollectionPointsQueries queries)
        {
            this._queries = queries;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollectionPointQueryItem>>> GetAllAsync()
        {
            var result = await this._queries.GetCollectionPointListAsync(
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
        public async Task<ActionResult<CollectionPointQueryItem>> GetOneAsync(
            string id)
        {
            var result = await this._queries.GetOneCollectionPointAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
    }
}
