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
			Associate data
			)
		{
			await Task.CompletedTask;

		}
		public async Task UpdateAssociateASync(
			Associate data
			)
		{
			await Task.CompletedTask;

		}
		public async Task DeleteAssociateASync(
			Guid associateId
			)
		{
			await Task.CompletedTask;

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
			Car data
			)
		{
			await Task.CompletedTask;

		}
		public async Task UpdateCarAsync(
			Car data
			)
		{
			await Task.CompletedTask;

		}
		public async Task DeleteCarASync(
			Guid carId
			)
		{
			await Task.CompletedTask;

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
		#endregion
	}
}
