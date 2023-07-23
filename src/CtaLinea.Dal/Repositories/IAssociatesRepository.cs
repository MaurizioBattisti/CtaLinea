using CtaLinea.Model.External;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public interface IAssociatesRepository
	{
		Task DeleteAssociateASync(Guid associateId);
		Task DeleteCarASync(Guid carId);
		Task<IList<Associate>> GetAllAssociatesAsync(IDbConnection conn, IDbTransaction tran);
		Task<IList<Car>> GetAssociateCarsAsync(Guid associateId, IDbConnection conn, IDbTransaction tran);
		Task InsertAssociateASync(Associate data);
		Task InsertCarAsync(Car data);
		Task UpdateAssociateASync(Associate data);
		Task UpdateCarAsync(Car data);
	}
}