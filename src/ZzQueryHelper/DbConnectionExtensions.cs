using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.QueryHelper
{
    public static class DbConnectionExtensions
    {
        public static async Task<QueryItemList<T>> QueryListAsync<T> (
            this IDbConnection connection,
            IQueryDefinition<T> querydef
            )
            where T: class
        {
            int totalRows = -1;
            var countQuery = querydef.GetCountQuery();
            if (string.IsNullOrEmpty(countQuery) == false )
            {
                totalRows = await connection.ExecuteScalarAsync<int>(
                    countQuery,
                    querydef.Arguments)
                    .ConfigureAwait(false);
            }

            var sql = querydef.GetSelectQuery();
            var aaa = await connection.QueryAsync(
				sql,
				querydef.Arguments)
				.ConfigureAwait(false);

			var data = await connection.QueryAsync<T>(
                sql,
                querydef.Arguments)
                .ConfigureAwait(false);

            return new QueryItemList<T>(
                data, 
                querydef.RequestedPage,
                totalRows);
        }

        public static async Task<T> QueryOneAsync<T>(
            this IDbConnection connection,
            IQueryDefinition<T> querydef
            )
            where T : class
        {
            return await connection.QuerySingleOrDefaultAsync<T>(
                querydef.GetSelectQuery(),
                querydef.Arguments)
                .ConfigureAwait(false);
        }
    }
}
