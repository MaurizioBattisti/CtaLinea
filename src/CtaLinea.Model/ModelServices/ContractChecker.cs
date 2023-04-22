using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.Runs;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.ModelServices
{
	public class ContractChecker 
		: IContractChecker
	{
		public async Task<CheckResult> CheckContractAsync(
			Contract contract)
		{
			var errors = new List<CheckResultItem>();
			var warnings = new List<CheckResultItem>();
			var informations = new List<CheckResultItem>();
			var result = new RunCheckResult()
			{
				Status = CheckStatus.Success,
				Errors = errors,
				Warnings = warnings,
				Informations = informations
			};

			if (string.IsNullOrEmpty(contract.ContractName) == true)
			{
				errors.Add(new CheckResultItem()
				{
					Category = string.Empty,
					Description = "Il nome identififativo dell'appalto non può essere vuoto",
					Title = string.Empty,
				});
			}
			if (string.IsNullOrEmpty(contract.ContractName) == false
				&& contract.ContractName.Length > 200)
			{
				errors.Add(new CheckResultItem()
				{
					Category = string.Empty,
					Description = "Il nome identififativo dell'appalto non può essere più lungo di 200 caratteri",
					Title = string.Empty,
				});
			}
			if (contract.StartDate > contract.EndDate)
			{
				errors.Add(new CheckResultItem()
				{
					Category = string.Empty,
					Description = "Le date di inizio e fien dell'appalto sono incongruenti",
					Title = string.Empty,
				});
			}

			// controlla se nella lsita di dettaglaio ci sono erorri
			if (errors.Count > 0) result.Status = CheckStatus.Failed;
			else if (warnings.Count > 0) result.Status = CheckStatus.Warning;
			else if (informations.Count > 0) result.Status = CheckStatus.Information;

			if (errors.Count > 0)
			{
				result.Title = "Errori";
				result.Description = "Errori nella definizione dell'appalto";
			}

			return await Task.FromResult(result);
		}
	}
}
