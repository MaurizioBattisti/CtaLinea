using CtaLinea.Model.ScheduledTasks;
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
    public class SchedulerTaskRepository
        : RepositoryBase
        , ISchedulerTaskRepository
    {
        private const string SQL_TagsTable = "[dbo].[SchedulerTasks]";

        private readonly CtaDbContext _context;
        private readonly ILogger _logger;

        public SchedulerTaskRepository(
            CtaDbContext context,
            ILogger<SchedulerTaskRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsertASync(
            ScheduledTaskItem model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            await this.InsertTableAsync(
                SQL_TagsTable,
                conn, null,
                this.GetData(model));
            return await this.GetIdentityAsync(conn);
        }
        public async Task UpdateASync(
            ScheduledTaskItem model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            await this.UpdateTableAsync(
                SQL_TagsTable,
                conn, null,
                this.GetKey(model.Id),
                this.GetData(model)
                );
        }
        public async Task DeleteASync(
            int id)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            await this.DeleteTableAsync(
                SQL_TagsTable,
                conn, null,
                this.GetKey(id)
                );
        }

        private object GetData(
            ScheduledTaskItem model)
        {
            return new
            {
                model.ActivityId,
                model.Frequency,
                model.RrequencyMask,

                model.StartTime,
                model.EndTime,
                model.Interval,

                model.Arguments,
                model.Timeout,

                model.Active
            };
        }
        private object GetKey(int id)
        {
            return new
            {
                Id = id
            };
        }

    }
}
