using CtaLinea.Model;
using CtaLinea.Model.Response;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Uility
{
	public class AddMultiRunElastibusDayRequestHandler
		: IRequestHandler<AddMultiRunElastibusDayRequest, OperationResponse>
	{
		private readonly CultureInfo _culture;
		private readonly IUtilityREpository _repo;
		private readonly ILogger _logger;

		public AddMultiRunElastibusDayRequestHandler (
			IUtilityREpository repo,
			ILogger<AddMultiRunElastibusDayRequestHandler> logger)
		{
			_culture = new CultureInfo("it-IT");
			_repo = repo;
			_logger = logger;
		}

		public async  Task<OperationResponse> Handle(
			AddMultiRunElastibusDayRequest request, 
			CancellationToken cancellationToken)
		{
			var items = this.IterateData(request.Data);

			var res = await _repo.AddElastibusDaysASync (
				items)
				.ConfigureAwait (false);

			return await Task.FromResult(res)
				.ConfigureAwait(false);
		}

		private IEnumerable<ElastibusDayDataItem> IterateData (
			string data)
		{
            if (string.IsNullOrEmpty (data) == false)
			{
                var rows = data.Split("\n");
                foreach (var row in rows)
				{
					if (string.IsNullOrWhiteSpace(row)) continue;
					var elements = row.Split("\t");

					// recupera l'id della corsa
					if (elements.Length < 3) continue;

					if (elements.Length == 3)
					{
						// imposta a zero il valore delle persone
						elements = new string[4] { elements[0], elements[1], elements[2], "0" };
					}

					bool ok = false;
					ok = int.TryParse(elements[0], NumberStyles.Integer, _culture, out int ctaId);

					if (ok == false) continue;
					ok = DateTime.TryParse(elements[1], _culture, DateTimeStyles.AssumeUniversal, out DateTime day);

					if (ok == false) continue;
					ok = float.TryParse(elements[2], NumberStyles.Number, _culture, out float km);

					if (ok == false) continue;
					ok = int.TryParse(elements[3], NumberStyles.Integer, _culture, out int peopleCount);

					if (ok == true)
					{
						yield return new ElastibusDayDataItem()
						{
							RunCtaId = ctaId,
							Date = day,
							Km = km,
							PeopleCount = peopleCount
						};
					}
				}
			}
		}
	}
}
