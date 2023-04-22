using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public interface IUtilityREpository
	{
		Task ChangeRunCarDataAsync(IDictionary<Guid, Guid> replacementMap, DateTime? refDate = null);
		Task<IEnumerable<Guid>> GetCarForDiscontinuationAsync(Guid associateId, DateTime? refDate = null);
	}
}