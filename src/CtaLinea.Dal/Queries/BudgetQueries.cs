using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using CtaLinea.QueryModel;
using ZzSoft.QueryHelper;
using CtaLinea.Model.QueryModel;
using ZzSoft.CtaLinea.Dal.Services;

namespace ZzSoft.CtaLinea.Dal.Queries
{
	public class BudgetQueries 
		: IBudgetQueries
	{
		private const string Budget_Table = "[dbo].[vw_Budget] b";

		private readonly CtaDbContext _context;
		private readonly ICurrentUserService _userSvc;
		private readonly IZzRequestConstx _zzContext;

		public BudgetQueries(
			ICurrentUserService userSvc,
			IZzRequestConstx zzContext,
			CtaDbContext context)
		{
			this._context = context;
			this._zzContext = zzContext;
			this._userSvc = userSvc;
		}

		public async Task<QueryItemList<BudgetQueryItem>> GetBudgetListAsync(
			IFilteringContext filterContext)
		{
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			string filter = idAss != null ? "b.AssociateId = @AssociateId" : null;
			object args = idAss != null ? new { AssociateId = idAss } : null;

			var queryDef = new QueryDefinition<BudgetQueryItem>(
				Budget_Table,
				filterContext,
				filter,
				args);

			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
			await conn.InitializeSession(this._zzContext);
			return await conn.QueryListAsync(
				queryDef)
				.ConfigureAwait(false);
		}
		public async Task<BudgetQueryItem> GetOneBudgetAsync(
			IFilteringContext filterContext,
			int id)
		{
			string filter = "b.BudgetId = @BudgetId"; ;
			object args = new { BudgetId = id };
			var idAss = await _userSvc.GetUserAssociateId().ConfigureAwait(false);
			filter += idAss != null ? "AND b.AssociateId = @AssociateId" : string.Empty;
			if (idAss != null)
			{
				args = new
				{
					BudgetId = id,
					AssociateId = idAss
				};
			}

			var queryDef = new QueryDefinition<BudgetQueryItem>(
				Budget_Table,
				filterContext,
				filter,
				args);

			using IDbConnection conn = this._context.GetNewConnection();
			conn.Open();
			await conn.InitializeSession(this._zzContext);
			return await conn.QueryOneAsync(
				queryDef)
				.ConfigureAwait(false);
		}

	}
}
