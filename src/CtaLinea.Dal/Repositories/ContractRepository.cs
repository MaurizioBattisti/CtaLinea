using CtaLinea.Model.Base;
using Microsoft.EntityFrameworkCore;
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
	public class ContractRepository
		: RepositoryBase
		, IContractRepository
	{
		private const string SQL_TagsTable = "[dbo].[Contracts]";

		private readonly CtaDbContext _context;
		private readonly ILogger _logger;

		public ContractRepository(
			CtaDbContext context,
			ILogger<ContractRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<int> InsertASync(
			Contract model)
		{
			using IDbConnection conn = this._context.Database.GetDbConnection();
			conn.Open();

			await this.InsertTableAsync(
				SQL_TagsTable,
				conn, null,
				this.GetTagData(model));
			return await this.GetIdentityAsync(conn);
		}
		public async Task UpdateASync(
			Contract model)
		{
			using IDbConnection conn = this._context.Database.GetDbConnection();
			conn.Open();

			await this.UpdateTableAsync(
				SQL_TagsTable,
				conn, null,
				this.GetTagKey(model.ContractId),
				this.GetTagData(model)
				);
		}
		public async Task DeleteASync(
			int tagId)
		{
			using IDbConnection conn = this._context.Database.GetDbConnection();
			conn.Open();

			await this.DeleteTableAsync(
				SQL_TagsTable,
				conn, null,
				this.GetTagKey(tagId)
				);
		}

		private object GetTagData(
			Contract model)
		{
			return new
			{
				model.ContractName,
				model.ContractDescription,
				model.StartDate,
				model.EndDate
			};
		}
		private object GetTagKey(int id)
		{
			return new
			{
				ContractId = id
			};
		}
	}
}
