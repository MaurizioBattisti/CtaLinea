using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.ModelServices;
using CtaLinea.Model.QueryModel;
using CtaLineaWebApi.Application.Commands.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;
using ZzSoft.CtaLinea.Dal.Queries;

namespace CtaLineaWebApi.Controllers
{
	[Authorize]
    [ApiController]
    [Route("api/contracts")]
    public class ContractsController
        : ZControllerBase
    {
        private readonly IContractsQueries _queries;
		private readonly ISender _mediator;
		private readonly IContractChecker _checker;

        public ContractsController(
			ISender mediator,
			IContractChecker checker,
			IContractsQueries queries)
        {
            this._mediator = mediator;
            this._checker = checker;
			this._queries = queries;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractQueryItem>>> GetAllAsync()
        {
            var result = await this._queries.GetContractListAsync(
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<ContractQueryItem>> GetOneAsync(
            int id)
        {
            var result = await this._queries.GetOneContractAsync(id)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }

		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPost]
		public async Task<ActionResult<int>> InsertAsync(
			[FromBody] Contract model)
		{
            var checkResult = await this._checker.CheckContractAsync(model);
			// controll il sirultato del check
			if (checkResult.Status == CheckStatus.Failed)
			{
				return this.BadRequest(checkResult);
			}

			try
			{
                var request = new InsertContractRequest()
                {
                    Model = model
                };
                var result = await this._mediator.Send(request)
                    .ConfigureAwait(false);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
				return Conflict(
					new ValidationProblemDetails(new Dictionary<string, string[]>())
					{
						Title = "Inserimetno contratto Fallito",
						Detail = ex.Message
					});
			}
		}

		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[HttpPut]
		[Route("{id}")]
		public async Task<ActionResult<int>> UpdateAsync(
			[FromRoute] int id,
            [FromBody] Contract model)
		{
			model.ContractId = id;
			var checkResult = await this._checker.CheckContractAsync(model);
			// controll il sirultato del check
			if (checkResult.Status == CheckStatus.Failed)
			{
				return this.BadRequest(checkResult);
			}

			try
			{
				var request = new UpdateContractRequest()
				{
					Model = model
				};
				var result = await this._mediator.Send(request)
					.ConfigureAwait(false);
				if (result.Success == false)
				{
					return this.NotFound();
				}

				return this.Ok(id);
			}
			catch (Exception ex)
			{
				return Conflict(
					new ValidationProblemDetails(new Dictionary<string, string[]>())
					{
						Title = "Aggiornamento contratto Fallito",
						Detail = ex.Message
					});
			}
		}
		[Consumes(MediaTypeNames.Application.Json)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status409Conflict)]
		[HttpDelete]
		[Route("{id}")]
		public async Task<ActionResult<ContractQueryItem>> DeleteAsync(
			[FromRoute] int id)
		{
			try
			{
				var request = new DeleteContractRequest()
				{
					ContractId = id
				};
				var result = await this._mediator.Send(request)
					.ConfigureAwait(false);
				if (result.Success == false)
				{
					return this.NotFound();
				}

				return this.NoContent();
			}
			catch (Exception ex)
			{
				return Conflict(
					new ValidationProblemDetails(new Dictionary<string, string[]>())
					{
						Title = "Eliminazione contratto fallita",
						Detail = ex.Message
					});
			}
		}

		[Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("periods")]
        public async Task<ActionResult<IEnumerable<OperationPeriodQueryItem>>> GetAllPeriodsAsync()
        {
            var result = await this._queries.GePOperatingPeriodstListAsync(
                this.FilteringContext)
                .ConfigureAwait(false);

            return await this.ModelOKAsync(result)
                .ConfigureAwait(false);
        }
    }
}
