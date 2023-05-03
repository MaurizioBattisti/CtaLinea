using CtaLinea.Model.Contab;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public interface IForfaitRepository
	{
		Task AddRunsASync(int id, IEnumerable<Guid> runIds);
		Task DeleteASync(int id);
		Task<int> InsertASync(MultiRunForfait model);
		Task RemoveAllRunsAsync(int id);
		Task RemoveRunsAsync(IEnumerable<Guid> runIds);
		Task UpdateASync(MultiRunForfait model);
	}
}