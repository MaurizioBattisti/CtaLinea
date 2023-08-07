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
    public class UsersQueries 
        : IUsersQueries
    {
        private const string Sql_UserTable = "[dbo].[vw_Users] u";

        private readonly CtaDbContext _context;
        private readonly IZzRequestConstx _zzContext;

        public UsersQueries(
            CtaDbContext context, 
            IZzRequestConstx zzContext)
        {
            this._context = context;
            _zzContext = zzContext; 
        }

        public async Task<QueryItemList<UserQueryModel>> GetListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<UserQueryModel>(
                Sql_UserTable,
                filterContext);

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<UserQueryModel> GetOneASync(
            string id)
        {
            var queryDef = new QueryDefinition<UserQueryModel>(
                Sql_UserTable,
                null,
                "u.UserName = @UserName",
                new { UserName = id });

            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }

    }
}
