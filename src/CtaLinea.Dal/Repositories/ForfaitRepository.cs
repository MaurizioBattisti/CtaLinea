using CtaLinea.Model.Contab;
using CtaLinea.Model.TaskRequest;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
	public class ForfaitRepository
		: RepositoryBase
		, IForfaitRepository
	{
		private const string Sql_ForfaitTable = "[dbo].[MultiRunForfait]";
		private const string SQl_ForfaitDetailTable = "[dbo].[MultiRunForfaitDetails]";

		private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;
        private readonly ILogger _logger;

		public ForfaitRepository(
			CtaDbContext context,
			IZzRequestConstx zzContext,
			ILogger<ForfaitRepository> logger)
		{
			_context = context;
			_zzContext = zzContext;
			_logger = logger;
		}

		public async Task<int> InsertASync(
			MultiRunForfait model)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            await this.InsertTableAsync(
				Sql_ForfaitTable,
				conn, null,
				this.GetData(model));
			return await this.GetIdentityAsync(conn);
		}
		public async Task UpdateASync(
			MultiRunForfait model)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            await this.UpdateTableAsync(
				Sql_ForfaitTable,
				conn, null,
				this.GetKey(model.ForfaitId),
				this.GetData(model)
				);
		}
		public async Task DeleteASync(
			int id)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            await this.DeleteTableAsync(
				Sql_ForfaitTable,
				conn, null,
				this.GetKey(id)
				);
		}

		public async Task AddRunsASync(
			int id,
			IEnumerable<Guid> runIds)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            var tran = conn.BeginTransaction();
			try
			{
				var argsPrefis = "p_";
				var inFilter = this.GetArgumetnNameListString(runIds.Count(), argsPrefis);
				var args = new ExpandoObject();
				var dict = args as IDictionary<string, object>;
				dict.Add("Id", id);
				int index = 0;
				foreach (var item in runIds)
				{
					dict.Add(
						this.GetArgumetnName(index, argsPrefis),
						item
						);
					++index;
				}

				// aggiorna tutti i dettagli con il run indicato asswegnandogli il valore di forfait
				// UPDATE {0} SET ForfaitID = @Id WHERE RunId IN ( {1} )
				var sql = string.Format("UPDATE {0} SET ForfaitID = @Id WHERE RunId IN ( {1} )",
					SQl_ForfaitDetailTable,
					inFilter);
				await conn.ExecuteAsync(
					sql,
					args,
					tran)
					.ConfigureAwait(false);

				// recupera tutti gli id
				sql = string.Format("SELECT RunId FROM {0} WHERE RunId IN ( {1} )",
					SQl_ForfaitDetailTable,
					inFilter);
				var updatedIds = await conn.QueryAsync<Guid> (
					sql,
					args,
					tran)
					.ConfigureAwait (false);
				
				// calcola la lista degli id da aggiungere
				var runIdsToAdd = runIds;
				if (updatedIds != null)
				{
					runIdsToAdd = runIds.Except(updatedIds);
				}
				
				// se ci sono id da aggiungere ...
				if (runIdsToAdd != null
					&& runIdsToAdd.Count() > 0)
				{
					// crea i nuovi argomenti
					args = new ExpandoObject();
					dict = args as IDictionary<string, object>;
					dict.Add("Id", id);
					index = 0;
					foreach (var item in runIdsToAdd)
					{
						dict.Add(
							this.GetArgumetnName(index, argsPrefis),
							item
							);
						++index;
					}
					// prepara il coamndo
					var insertValues = string.Join(",", 
						this.GetArgumetnNameList(index, argsPrefis, "(@Id, {0})")
						);

					// prepara il cmando di inser ...
					sql = string.Format("INSERT INTO {0} (ForfaitId, RunId) VALUES {1}",
						SQl_ForfaitDetailTable,
						insertValues
						);

					// e li aggiunge
					await conn.ExecuteAsync(
						sql,
						args,
						tran)
						.ConfigureAwait(false);
				}

				tran.Commit();
				tran = null;
			}
			finally
			{
				if (tran != null) tran.Rollback();
			}

			await Task.CompletedTask;
		}

		public async Task RemoveRunsAsync(
			IEnumerable<Guid> runIds)
		{
			var argsPrefis = "p_";
			var inFilter = this.GetArgumetnNameListString(runIds.Count(), argsPrefis);
			var args = new ExpandoObject();
			var dict = args as IDictionary<string, object>;
			int index = 0;
			foreach (var item in runIds)
			{
				dict.Add(
					this.GetArgumetnName(index, argsPrefis),
					item
					);
				++index;
			}
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            var deleteValues = string.Join(",",
                this.GetArgumetnNameList(index, argsPrefis)
                );

            // elimina tutit i dettagli dei forfait in cui la corsa è in lista
            var sql = string.Format("DELETE FROM {0} WHERE RunId IN ( {1} )",
				SQl_ForfaitDetailTable,
                deleteValues
                );

			// e li aggiunge
			await conn.ExecuteAsync(
				sql,
				args)
				.ConfigureAwait(false);
		}
		public async Task RemoveAllRunsAsync(
			int id)
		{
			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
            await conn.InitializeSession(this._zzContext);

            // elimina tutti i dettagli del forfait il cui id è indicato
            await this.DeleteTableAsync(
				SQl_ForfaitDetailTable,
				conn, null,
				this.GetKey(id)
				);
		}

		private object GetData(
			MultiRunForfait model)
		{
			return new
			{
				model.ContractId,
				model.ForfaitName,
				model.ForfaitType,
				model.Amount
			};
		}
		private object GetKey(int id)
		{
			return new
			{
				ForfaitId = id
			};
		}

	}
}
