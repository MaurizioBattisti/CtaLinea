using CtaLinea.Model.Filters;
using CtaLinea.Model.QueryModel;
using CtaLinea.QueryModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.QueryHelper;
using Dapper;
using System.Globalization;

namespace ZzSoft.CtaLinea.Dal.Queries
{
	public class RunQueries
		: IRunQueries
	{
		private const string RunItemListSql_Table = "[dbo].[vw_Runs] r";
        private const string RunItemListSql_AdvandedFilters = @"
 INNER JOIN dbo.tvf_Runs_AdvancedFilter(@AssociateId, @CarId, @MinSittings
, @MaxSittings, @LineNumber, @RunNumber, @Node, @StartDate, @EndDate, @StartTime, @EndTime, @Frequency, @CalendarIds
, @WeekDays, @InContract, @ActiveRun, @DateRef, @TabIds, @ForfaitId, @CollectionPointId) a ON a.RunId = r.RunId";
        private const string RunVariationList_Table = "[dbo].[vw_RunVariations] v";

        private CtaDbContext _context;

		public RunQueries(
			CtaDbContext context)
		{
			this._context = context;
		}

		public async Task<QueryItemList<RunItemQueryModel>> GetRunListAsycn(
            IFilteringContext filterContext,
            RunAdvancedFilters advancedFilter = null
            )
		{
			object args = null;
			string table = RunItemListSql_Table;

            using IDbConnection conn = this._context.Database.GetDbConnection();

            // filtri standard
            string stdFilters = string.Empty;
            if (advancedFilter != null)
            {
                if (advancedFilter.ContractId != null)
                {
                    stdFilters += "r.ContractId = @ContractId";
                }
                if (advancedFilter.StartPeriod != null)
                {
                    if (stdFilters.Length > 0) stdFilters += " AND ";
                    stdFilters += "COALESCE(r.EndDate, r.ctrEndDAte) >= @StartPeriod";
                }
                if (advancedFilter.EndPeriod != null)
                {
                    if (stdFilters.Length > 0) stdFilters += " AND ";
                    stdFilters += "COALESCE(r.StartDate, r.ContractStart) <= @EndPeriod";
                }
                args = new
                {
                    ContractId = advancedFilter.ContractId,
                    StartPeriod = advancedFilter.StartPeriod,
                    EndPeriod = advancedFilter.EndPeriod
                };
            }

            if (advancedFilter != null
                && advancedFilter.HasImpact())
            {
                // controlla se  prima di fare la ricerca deve anche eseguire un ricalcolo dei calendari
                if (advancedFilter.StartDate != null
                    || advancedFilter.EndDate != null
                    || advancedFilter.WeekDays != null)
                {
					// esegue un ricalcolo dei giorni
					await conn.ExecuteAsync(
					    sql: "[dbo].[uo_RecalcRunDays_Massive]",
						 param: null,
						 transaction: null,
						 commandType: CommandType.StoredProcedure,
                         commandTimeout: 600
						 );
				}

                table += RunItemListSql_AdvandedFilters;
				args = new
				{
                    AssociateId = advancedFilter.AssociateId,
                    CarId = advancedFilter.CarId,
                    MinSittings = advancedFilter.MinSittings,
                    MaxSittings = advancedFilter.MaxSittings,
                    LineNumber = advancedFilter.LineNumber,
                    RunNumber = advancedFilter.RunNumber,
                    Node = advancedFilter.Node,
                    StartDate = advancedFilter.StartDate,
                    EndDate = advancedFilter.EndDate,
                    StartTime = advancedFilter.StartTime,
                    EndTime = advancedFilter.EndTime,
                    Frequency = advancedFilter.Frequency,
                    CalendarIds = advancedFilter.CalendarIds != null ? string.Join(",", advancedFilter.CalendarIds) : (string) null,
                    WeekDays = advancedFilter.WeekDays != null ? string.Join(",", advancedFilter.WeekDays) : (string)null,
                    InContract = advancedFilter.InContract,
                    ActiveRun = advancedFilter.ActiveRun,
                    DateRef = advancedFilter.DateRef,
                    TabIds = advancedFilter.TabIds != null ? string.Join(",", advancedFilter.TabIds) : (string)null,

                    ContractId = advancedFilter.ContractId,
                    StartPeriod = advancedFilter.StartPeriod,
                    EndPeriod = advancedFilter.EndPeriod,
                    ForfaitId = advancedFilter.ForfaitId,
                    CollectionPointId = advancedFilter.CollectionPointId
                };
            }

            // se non sono stati aggiunti filtri ne aggiunge uno ininfluente
            if (string.IsNullOrEmpty(stdFilters) == true) stdFilters = "1 = 1";

            var queryDef = new QueryDefinition<RunItemQueryModel>(
				table,
				filterContext,
                stdFilters,
				args);

			return await conn.QueryListAsync(
				queryDef)
				.ConfigureAwait(false);
		}
		public async Task<RunItemQueryModel> GetOneRunAsync(
			Guid id)
		{
			var queryDef = new QueryDefinition<RunItemQueryModel>(
				RunItemListSql_Table,
				null,
                "r.RunId = @RunId",
				new { RunId = id });

			IDbConnection conn = this._context.Database.GetDbConnection();
			return await conn.QueryOneAsync(
				queryDef)
				.ConfigureAwait(false);
		}

        public async Task<QueryItemList<RunVariationQueryModel>> GetRunVariationsAsync(
            Guid runId,
            IFilteringContext filterContext)
		{
            var queryDef = new QueryDefinition<RunVariationQueryModel>(
                RunVariationList_Table,
                filterContext,
				"v.RunId = @RunId",
				new { RunId = runId });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<RunVariationQueryModel> GetOneVariationAsync(
            Guid id)
		{
            var queryDef = new QueryDefinition<RunVariationQueryModel>(
                RunVariationList_Table,
                null,
                "v.RunVariationId = @RunVariationId",
                new { RunVariationId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);

        }

    }
}
