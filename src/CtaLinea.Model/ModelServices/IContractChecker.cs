using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;

namespace CtaLinea.Model.ModelServices
{
	public interface IContractChecker
	{
		Task<CheckResult> CheckContractAsync(Contract contract);
	}
}