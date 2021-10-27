using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;
using ZzSoft.CtaLinea.Dal.QueryModel;

namespace CtaLineaWebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/importlogs")]
    public class ImportLogController
        : ZControllerBase
    {
        private readonly IImportQueries _queries;

        public ImportLogController(
            IImportQueries queries)
        {
            this._queries = queries;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportQueryItem>>> GetAllAsync()
        {
            var result = await this._queries.GetImportListAsync(
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        // singolo import
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ImportQueryItem>> GetOneAsync(
            Guid id)
        {
            var result = await this._queries.GetOneImportAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }

        // ultimo import di un determinato tipo
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("pending/{importDescr}")]
        public async Task<ActionResult<ImportQueryItem>> GetLastPendingAsync(
            string importDescr)
        {
            var result = await this._queries.GetLastPendingImportAsync(importDescr)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }

        // dettagli di un import
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("{id}/details")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImportDetailQueryItem>>> GetAllDetailAsync(
            Guid id)
        {
            var result = await this._queries.GetImportDetailListAsync(
                id,
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }

        // un singolo dettaglio di un import
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}/details/{serviceId}")]
        public async Task<ActionResult<ImportDetailQueryItem>> GetOneDetailAsync(
            Guid id,
            int serviceId)
        {
            var result = await this._queries.GetOneImportDetailAsync(id, serviceId)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
    }
}
