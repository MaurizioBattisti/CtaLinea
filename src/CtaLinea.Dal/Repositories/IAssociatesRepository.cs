using CtaLinea.Model.External;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public interface IAssociatesRepository
	{
		Task DeleteAssociateASync(
            Guid associateId,
            IDbConnection conn,
            IDbTransaction tran);
		Task DeleteCarASync(
            Guid carId,
            IDbConnection conn,
            IDbTransaction tran);
		Task<IList<Associate>> GetAllAssociatesAsync(IDbConnection conn, IDbTransaction tran);
		Task<IList<Car>> GetAssociateCarsAsync(Guid associateId, IDbConnection conn, IDbTransaction tran);
		Task InsertAssociateASync(
            Associate data,
            IDbConnection conn,
            IDbTransaction tran);
		Task InsertCarAsync(
            Car data,
            IDbConnection conn,
            IDbTransaction tran);
		Task UpdateAssociateASync(
            Associate data,
            IDbConnection conn,
            IDbTransaction tran);
		Task UpdateCarAsync(Car 
            data,
            IDbConnection conn,
            IDbTransaction tran);

        Task<IList<Driver>> GetAssociateDriversAsync(
            Guid associateId,
            IDbConnection conn,
            IDbTransaction tran);
        Task InsertDriverAsync(
            Driver data,
            IDbConnection conn,
            IDbTransaction tran);
        Task UpdateDriverAsync(
            Driver data,
            IDbConnection conn,
            IDbTransaction tran);
        Task DeleteDriverASync(
            Guid driverId,
            IDbConnection conn,
            IDbTransaction tran);
    }
}