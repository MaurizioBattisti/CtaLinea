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
    public class UserRepository 
        : IUserRepository
    {
        private readonly ILogger _logger;
        private readonly CtaDbContext _context;

        public UserRepository(
            CtaDbContext context,
            ILogger<UserRepository> logger
            )
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<UserEntity> GetUserAsync(
            string userName)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            string sql = "SELECT * FROM dbo.Meta_Users WHERE UserName = @UserName";
            var user = await conn.QuerySingleOrDefaultAsync<UserEntity>(
                sql,
                new { UserName = userName }
                );

            if (user != null)
            {
                // legge i dati dei ruoli
                var roles = await this.GetUserRolesAsync(userName, conn);
                user.Roles = roles;
            }
            return user;
        }

        public async Task<UserEntity> UpdateUserAsync(
            UserEntity user
            )
        {
            using (IDbConnection conn = this._context.Database.GetDbConnection())
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                string sql = "UPDATE dbo.Meta_Users SET PasswordHash = @PasswordHash, Description = @Description, Email = @Email, MustChangePAssword = @MustChangePAssword, Expiration = @Expiration, AssociateId = @AssociateId  WHERE UserName = @UserName";
                var rows = await conn.ExecuteAsync(
                    sql,
                    user,
                    tran
                    );
                if (rows > 0
                    && user.Roles != null)
                {
                    // recupera i ruoli per vedere se deve aggioranrli
                    var roles = await this.GetUserRolesAsync(user.UserName, conn, tran);

                    if (roles != null)
                    {

                        // trova i ruoli da eliminare
                        var rolesToDelete = roles.Where(
                            r => user.Roles.Contains(r) == false
                            );

                        // trova i ruoli da aggiungere
                        var rolesToAdd = user.Roles.Where(
                            r => roles.Contains(r) == false
                            );

                        // elimina i ruoli in eccesso
                        foreach (var r in rolesToDelete)
                        {
                            await conn.ExecuteAsync(
                                "DELETE FROM dbo.Met_Roles WHERE UserName = @UserName AND RoleId = @RoleId",
                                new
                                {
                                    UserName = user.UserName,
                                    RoleId = r
                                },
                                tran);
                        }

                        // aggiunge i nuovi ruoi
                        foreach (var r in rolesToAdd)
                        {
                            await conn.ExecuteAsync(
                                "INSERT INTO dbo.Met_Roles VALUES (@UserName, @RoleId)",
                                new
                                {
                                    UserName = user.UserName,
                                    RoleId = r
                                },
                                tran);
                        }
                    }
                }
                tran.Commit();
            }

            return await this.GetUserAsync(user.UserName); ;
        }

        private async Task<IEnumerable<string>> GetUserRolesAsync(
            string userName,
            IDbConnection conn,
            IDbTransaction tran = null)
        {
            string sqlRoles = "SELECT RoleId FROM dbo.Meta_Roles WHERE UserName = @UserName";
            var roles = await conn.QueryAsync<string>(
                sqlRoles,
                new { UserName = userName },
                tran);
            return roles;
        }
    }
}
