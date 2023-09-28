using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Utilities;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.Model;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public class UtilityREpository
		: RepositoryBase
		, IUtilityREpository
	{
		private readonly CtaDbContext _context;
		private readonly ILogger _logger;
        private readonly IZzRequestConstx _zzContext;

        public UtilityREpository(
			CtaDbContext context,
			IZzRequestConstx zzzContext,
			ILogger<UtilityREpository> logger)
		{
			_context = context;
			_zzContext = zzzContext;
			_logger = logger;
		}

		public async Task<IEnumerable<Guid>> GetCarForDiscontinuationAsync(
			Guid associateId,
			DateTime? refDate = null)
		{
			if (refDate == null) refDate = DateTime.Today;

			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            using var reader = await conn.QueryMultipleAsync(
                "[dbo].[up_GetCarForDiscontinuation]",
				param: new
				{
					AssociateId = associateId,
					RefDate = refDate
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			var items = reader.Read<Guid>();
			return await Task.FromResult(items);
		}

		public async Task ChangeRunCarDataAsync(
			IDictionary<Guid, Guid> replacementMap,
			DateTime? refDate = null
			)
		{
			// trasforma lil dictionary di mappatura in una stringa di mappatura
			string map = string.Join(",", replacementMap.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));

			await Task.CompletedTask;

			// TODO: chiamare la stored proc [dbo].[up_ChangeCarsFromDate]
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            await conn.ExecuteAsync(
				"[dbo].[up_ChangeCarsFromDate]",
				param: new
				{
					RefDate = refDate,
					CarMapList = map
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			return;
		}

		public async Task<IEnumerable<RunIncongruenceModel>> GetRunIncongruenceASync (
			Guid? runId = null,
			DateTime? startDate = null,
			DateTime? endDAte = null,
			IEnumerable<int> whatIncongruence = null
            )
		{
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            string whatToCheck = null;
			if (whatIncongruence != null
				&& whatIncongruence.Count() > 0)
			{
				whatToCheck = string.Join(",", whatIncongruence);
            }

            var items = await conn.QueryAsync<RunIncongruenceModel>(
                "[dbo].[up_GetRunIncongruence]",
                param: new
                {
                    RunId = runId,
                    StartDate = startDate,
                    EndDAte = endDAte,
                    WhatToCheck = whatToCheck
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return await Task.FromResult(items);
        }

		public async Task <IEnumerable <ElastibusDayExport>> GetElastibusDayForExportAsyunc (
			DateTime? startDate,
			DateTime? endDate,
			int? contractId
			)
		{
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            var items = await conn.QueryAsync<ElastibusDayExport>(
                "[dbo].[up_GetElastibusDayForExport]",
                param: new
                {
                    ContractId = contractId,
                    StartDate = startDate,
                    EndDAte = endDate
                },
                commandType: CommandType.StoredProcedure,
                commandTimeout: 600);

            return await Task.FromResult(items);
        }
		public async Task<IEnumerable<ExportRunTt>> GetExportTtAsync(
			DateTime? startDate,
			DateTime? endDate,
			int? contractId
			)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
			await conn.InitializeSession(this._zzContext);

			var items = await conn.QueryAsync<ExportRunTt>(
				"[dbo].[up_GetRunsForExportTt]",
				param: new
				{
					ContractId = contractId,
					StartDate = startDate,
					EndDAte = endDate
				},
				commandType: CommandType.StoredProcedure,
				commandTimeout: 600);

			return await Task.FromResult(items);
		}
	}
}
