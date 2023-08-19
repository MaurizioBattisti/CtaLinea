using CtaLinea.Model.External;
using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public class AssociatesRepository
		: RepositoryBase
		, IAssociatesRepository
	{
		private const string SQL_Associate_Table = "[dbo].[Associates]";
		private const string SQL_Car_Table = "[dbo].[Cars]";
        private const string SQL_Driver_Table = "[dbo].[Drivers]";
		private const string Sql_VirutalDelete_Fmt = "UPDATE {0} SET {1} = 0 WHERE {2} = @{2}";

        private readonly CtaDbContext _context;
		private readonly ILogger _logger;

		public AssociatesRepository(
			CtaDbContext context,
			ILogger<AssociatesRepository> logger
			)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<IList<Associate>> GetAllAssociatesAsync(
			IDbConnection conn,
			IDbTransaction tran)
		{
			var model = await conn.QueryAsync<Associate>(
				SQL_Select_Star + SQL_Associate_Table,
				null,
				tran
				).ConfigureAwait(false);

			return await Task.FromResult(model.ToList());
		}
		public async Task InsertAssociateASync(
			Associate data,
			IDbConnection conn,
            IDbTransaction tran
            )
		{
			await this.InsertTableAsync(
				SQL_Associate_Table,
				conn, tran,
				this.JoinObjects(
					this.GetAssociateKey(data.AssociateId),
					this.GetAssociateData(data)
				))
				.ConfigureAwait (false);
		}
		public async Task UpdateAssociateASync(
			Associate data,
            IDbConnection conn,
            IDbTransaction tran
            )
		{
			await this.UpdateTableAsync(
				SQL_Associate_Table,
				conn, tran,
				this.GetAssociateKey(data.AssociateId),
				this.GetAssociateData(data)
				).ConfigureAwait(false);
		}
		public async Task DeleteAssociateASync(
			Guid associateId,
            IDbConnection conn,
            IDbTransaction tran
            )
		{
			var sql = string.Format(Sql_VirutalDelete_Fmt,
				SQL_Associate_Table,
				"Active",
				"AssociateId");

			await conn.ExecuteAsync( 
				sql,
				new {AssociateId = associateId },
				tran)
				.ConfigureAwait(true);
		}

		public async Task<IList<Car>> GetAssociateCarsAsync(
			Guid associateId,
			IDbConnection conn,
			IDbTransaction tran)
		{
			var model = await conn.QueryAsync<Car>(
				SQL_Select_Star + SQL_Car_Table
				 + "WHERE AssociateId = @AssociateId",
				new { AssociateId = associateId },
				tran
				).ConfigureAwait(false);

			return await Task.FromResult(model.ToList());
		}

		public async Task InsertCarAsync(
			Car data,
            IDbConnection conn,
            IDbTransaction tran
            )
		{
            await this.InsertTableAsync(
                SQL_Car_Table,
                conn, tran,
                this.JoinObjects(
                    this.GetCarKey(data.CarId),
	                this.GetCarData(data)
				))
                .ConfigureAwait(false);
        }
        public async Task UpdateCarAsync(
			Car data,
            IDbConnection conn,
            IDbTransaction tran
            )
		{
            await this.UpdateTableAsync(
                SQL_Car_Table,
                conn, tran,
                this.GetCarKey(data.CarId),
                this.GetCarData(data)
                ).ConfigureAwait(false);
        }
        public async Task DeleteCarASync(
			Guid carId,
            IDbConnection conn,
            IDbTransaction tran
            )
		{
            var sql = string.Format(Sql_VirutalDelete_Fmt,
                SQL_Car_Table,
                "Active",
                "CarId");

            await conn.ExecuteAsync(
                sql,
                new { CarId = carId },
                tran)
                .ConfigureAwait(true);
        }

        public async Task<IList<Driver>> GetAssociateDriversAsync(
            Guid associateId,
            IDbConnection conn,
            IDbTransaction tran)
        {
            var model = await conn.QueryAsync<Driver>(
                SQL_Select_Star + SQL_Driver_Table
                 + "WHERE AssociateId = @AssociateId",
                new { AssociateId = associateId },
                tran
                ).ConfigureAwait(false);

            return await Task.FromResult(model.ToList());
        }

        public async Task InsertDriverAsync(
            Driver data,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.InsertTableAsync(
                SQL_Driver_Table,
                conn, tran,
                this.JoinObjects(
                    this.GetDriverKey(data.DriverId),
					this.GetDriverData(data)
				))
                .ConfigureAwait(false);
        }
        public async Task UpdateDriverAsync(
            Driver data,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            await this.UpdateTableAsync(
                SQL_Driver_Table,
                conn, tran,
                this.GetDriverKey(data.DriverId),
                this.GetDriverData(data)
                ).ConfigureAwait(false);
        }
        public async Task DeleteDriverASync(
            Guid driverId,
            IDbConnection conn,
            IDbTransaction tran
            )
        {
            var sql = string.Format(Sql_VirutalDelete_Fmt,
                SQL_Driver_Table,
                "Active",
                "DriverId");

            await conn.ExecuteAsync(
                sql,
                new { DriverId = driverId },
                tran)
                .ConfigureAwait(true);
        }

        #region data preparation
        private object GetAssociateKey(
			Guid associateId)
		{
			return new
			{
				AssociateId = associateId
			};
		}
		private object GetAssociateData(
			Associate data)
		{
			return new
			{
				data.Description,
				data.BsCustomerCode,
				data.BsSupplierCode,
				data.Active,
				data.Email
			};
		}

		private object GetCarKey(
			Guid carId)
		{
			return new
			{
				CarId = carId
			};
		}
		private object GetCarData(
			Car data)
		{
			return new
			{
				data.AssociateId,
				data.Description,
				data.RegNumber,
				data.NrSittings,
				data.BsCarId,

				data.PrimaryCar,
				data.SpareCar,

				data.ChassisNumber,

				data.FirstRegistration,
				data.DiscontinuationDate,
				data.Active,
			};
		}
        private object GetDriverKey(
            Guid driverId)
        {
            return new
            {
                DriverId = driverId
            };
        }
        private object GetDriverData(
            Driver data)
        {
            return new
            {
				data.AssociateId,
				data.BsDriverId,
				data.FirstName,
				data.LastName,
				data.LicenseNumber,
				data.LicenceCategory,
				data.DismissionDate,
                data.Active,
            };
        }
        #endregion
    }
}
