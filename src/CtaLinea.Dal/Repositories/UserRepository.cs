using CtaLinea.Model.Base;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
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
        private readonly IZzRequestConstx _zzContext;

        public UserRepository(
            CtaDbContext context,
            IZzRequestConstx zzContext,
            ILogger<UserRepository> logger
            )
        {
            this._context = context;
            this._zzContext = zzContext;
            this._logger = logger;
        }

        public async Task<UserEntity> GetUserAsync(
            string userName)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

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

        /*
        public async Task<UserEntity> UpdateUserAsync(
            UserEntity user
            )
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            var tran = conn.BeginTransaction();
            string sql = "UPDATE dbo.Meta_Users SET PasswordHash = @PasswordHash, Description = @Description, Email = @Email, MustChangePassword = @MustChangePassword, Expiration = @Expiration, AssociateId = @AssociateId, Interactive = @Interactive  WHERE UserName = @UserName";
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

            return await this.GetUserAsync(user.UserName); ;
        }
        */

        public async Task<UserEntity> InsertUSerAsync (
			UserEntity user)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            string rolesString = null;
            if (user.Roles != null)
            {
                rolesString = string.Join(",", user.Roles);
			}

			await conn.ExecuteAsync(
                "[dbo].[up_User_New]",
                new
                {
					UserName = user.UserName,
					PasswordHash = user.PasswordHash,
					Description = user.Description,
					Email = user.Email,
					Expiration = user.Expiration,
					MustChangePassword = user.MustChangePassword,
					Interactive = user.Interactive,
					AssociateId = user.AssociateId,
					Roles = rolesString
				},
                commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return await this.GetUserAsync(user.UserName); ;
        }
        public async Task<UserEntity> UpdateUserAsync(
            UserEntity user)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            string rolesString = null;
			if (user.Roles != null)
			{
				rolesString = string.Join(",", user.Roles);
			}

			await conn.ExecuteAsync(
                "[dbo].[up_User_Edit]",
				new
				{
					UserName = user.UserName,
					Description = user.Description,
					Email = user.Email,
					Expiration = user.Expiration,
					MustChangePassword = user.MustChangePassword,
					AssociateId = user.AssociateId,
					Roles = rolesString
				},
                commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return await this.GetUserAsync(user.UserName); ;
        }
        public async Task DeleteUserAsync(
            string userName)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            await conn.ExecuteAsync(
                "[dbo].[up_User_Delete]",
                new
                {
                    UserName = userName
                },
                commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
            return;
        }
        public async Task<UserEntity> SetUserPasswordAsync(
            string userName,
            string passwordHash,
            bool setMustChange = false)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            await conn.ExecuteAsync(
                "[dbo].[up_User_ResetPassword]",
                new
                {
                    UserName = userName,
                    PasswordHash = passwordHash,
                    SetMustChange = setMustChange
                },
                commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            return await this.GetUserAsync(userName); ;
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

        public async Task<bool> IsUserAssociateRun(
            string userName,
            Guid runId)
        {
            using IDbConnection conn = this._context.GetNewConnection();
            conn.Open();
            await conn.InitializeSession(this._zzContext);

            var data = await conn.ExecuteScalarAsync<bool>(
                "SELECT [dbo].[fn_IsAssociateUserRun](@UserName, @RunId)",
                new
                {
                    UserName = userName,
                    RunId = runId
                },
                commandType: CommandType.Text)
                .ConfigureAwait(false);
            return data;
        }
    }
}
