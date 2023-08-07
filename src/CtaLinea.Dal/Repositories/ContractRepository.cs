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
        private readonly IZzRequestConstx _zzContext;

        public ContractRepository(
			CtaDbContext context,
			IZzRequestConstx zzContext,
			ILogger<ContractRepository> logger)
		{
			_context = context;
			_zzContext = zzContext;
			_logger = logger;
		}

		public async Task<int> InsertASync(
			Contract model)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            await this.InsertTableAsync(
				SQL_TagsTable,
				conn, null,
				this.GetData(model));
			return await this.GetIdentityAsync(conn);
		}
		public async Task UpdateASync(
			Contract model)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            await this.UpdateTableAsync(
				SQL_TagsTable,
				conn, null,
				this.GetKey(model.ContractId),
				this.GetData(model)
				);
		}
		public async Task DeleteASync(
			int tagId)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            await this.DeleteTableAsync(
				SQL_TagsTable,
				conn, null,
				this.GetKey(tagId)
				);
		}

		private object GetData(
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
		private object GetKey(int id)
		{
			return new
			{
				ContractId = id
			};
		}
	}
}
