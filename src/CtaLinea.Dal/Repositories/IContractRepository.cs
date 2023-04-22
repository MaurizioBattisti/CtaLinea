using CtaLinea.Model.Base;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public interface IContractRepository
	{
		Task DeleteASync(int tagId);
		Task<int> InsertASync(Contract model);
		Task UpdateASync(Contract model);
	}
}