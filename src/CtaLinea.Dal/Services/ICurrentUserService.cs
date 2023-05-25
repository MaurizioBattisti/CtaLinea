using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Services
{
	public interface ICurrentUserService
	{
		Task<Guid?> GetUserAssociateId();
	}
}
