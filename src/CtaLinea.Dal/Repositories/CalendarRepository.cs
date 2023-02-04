using CtaLinea.Model.Calendar;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class CalendarRepository
        : RepositoryBase, ICalendarRepository
    {
        private const string Sql_Calendars_Table = "[dbo].[Calendars]";
        private const string Sql_CalendarPeriods_Table = "[dbo].[CalendarPeriods]";
        private const string Sql_CalendarHolidays_Table = "[dbo].[CalendarHolidays]";

        private const string SQL_Select_Star = "SELECT * FROM ";
        private const string Sql_SelectIdentity = "SELECT @@IDENTITY";

        private readonly CtaDbContext _context;
        private readonly ILogger _logger;

        public CalendarRepository(
            CtaDbContext context,
            ILogger<CalendarRepository> logger
            )
        {
            _context = context;
            _logger = logger;
        }

        #region calendars
        public async Task<Calendar> GetOneAsync(
            int calendarId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            return await this.InternalGetOneAsync(
                conn, null,
                calendarId);
        }
        public async Task<Calendar> UpdateAsync(
            Calendar model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var tran = conn.BeginTransaction();
            try
            {
                await this.UpdateTableAsync(
                    Sql_Calendars_Table,
                    conn, tran,
                    this.GetCalendarKey(model.CalendarId),
                    this.GetCalendarData(model)
                    );
                var data = await this.InternalGetOneAsync(conn, tran, model.CalendarId);
                tran.Commit();
                tran = null;
                return data;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        public async Task<Calendar> InsertAsync(
            Calendar model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var tran = conn.BeginTransaction();
            try
            {
                await this.InsertTableAsync(
                    Sql_Calendars_Table,
                    conn, tran,
                    this.JoinObjects(
                        this.GetCalendarData(model)
                        )
                    );
                var id = await conn.ExecuteScalarAsync<int>(Sql_SelectIdentity, transaction: tran);
                var data = await this.InternalGetOneAsync(conn, tran, id);
                tran.Commit();
                tran = null;
                return data;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        public async Task DeleteAsync(
            int calendarId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var tran = conn.BeginTransaction();
            try
            {
                // elimina tutta una corsa
                await this.DeleteTableAsync(
                    Sql_Calendars_Table,
                    conn, tran,
                    this.GetCalendarKey(calendarId)
                    );

                tran.Commit();
                tran = null;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        private async Task<Calendar> InternalGetOneAsync(
            IDbConnection conn,
            IDbTransaction tran,
            int calendarId)
        {
            var model = await conn.QueryFirstOrDefaultAsync<Calendar>(
                SQL_Select_Star + Sql_Calendars_Table
                    + " WHERE CalendarId = @CalendarId",
                this.GetCalendarKey(calendarId),
                tran
                );
            return await Task.FromResult(model);
        }
        private object GetCalendarData(
            Calendar model)
        {
            return new
            {
                BaseCalendarId = model.BaseCalendarId,
                CalendarName = model.CalendarName,
                CalendarType = model.CalendarType,
                Sundays = model.Sundays,
                PreHolyday = model.PreHolyday,
                PostHolyday = model.PostHolyday
            };
        }
        private object GetCalendarKey(
            int calendarId)
        {
            return new
            {
                CalendarId = calendarId
            };
        }
        #endregion

        #region calendar periods
        public async Task<IEnumerable<CalendarPeriod>> GetPeriodListAsync(
            int calendarId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var model = await conn.QueryAsync<CalendarPeriod>(
                SQL_Select_Star + Sql_CalendarPeriods_Table
                    + " WHERE CalendarId = @CalendarId",
                new
                {
                    CalendarId = calendarId
                });
            return await Task.FromResult(model);
        }
        public async Task<CalendarPeriod> GetOnePeriodAsync(
            int calendarPeriodId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            return await this.InternalGetOnePeriodAsync(conn, null, calendarPeriodId);
        }
        public async Task<CalendarPeriod> UpdatePeriodASync(
            CalendarPeriod model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var tran = conn.BeginTransaction();
            try
            {
                await this.UpdateTableAsync(
                    Sql_CalendarPeriods_Table,
                    conn, tran,
                    this.GetPeriodKey(model.CalendarPeriodId),
                    this.GetPeriodData(model)
                    );
                var data =  await this.InternalGetOnePeriodAsync(conn, tran, model.CalendarPeriodId);
                tran.Commit();
                tran = null;
                return data;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        public async Task<CalendarPeriod> InsertPeriodASync(
            CalendarPeriod model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var tran = conn.BeginTransaction();
            try
            {
                await this.InsertTableAsync(
                    Sql_CalendarPeriods_Table,
                    conn, tran,
                    this.JoinObjects(
                        this.GetPeriodData(model)
                        )
                    );
                var id = await conn.ExecuteScalarAsync<int>(Sql_SelectIdentity, transaction: tran);
                var data = await this.InternalGetOnePeriodAsync(conn, tran, id);
                tran.Commit();
                tran = null;
                return data;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        public async Task DeletePeriodAsync(
            int calendarPeriodId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            var tran = conn.BeginTransaction();
            try
            {
                // elimina tutta una corsa
                await this.DeleteTableAsync(
                    Sql_CalendarPeriods_Table,
                    conn, tran,
                    this.GetPeriodKey(calendarPeriodId)
                    );

                tran.Commit();
                tran = null;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }

        private async Task<CalendarPeriod> InternalGetOnePeriodAsync(
            IDbConnection conn,
            IDbTransaction tran,
            int periodId)
        {
            var model = await conn.QueryFirstOrDefaultAsync<CalendarPeriod>(
                SQL_Select_Star + Sql_CalendarPeriods_Table
                    + " WHERE CalendarPeriodId = @CalendarPeriodId",
                this.GetPeriodKey(periodId),
                tran);
            return await Task.FromResult(model);
        }
        private object GetPeriodData(
            CalendarPeriod model)
        {
            return new
            {
                CalendarId = model.CalendarId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Note = model.Note
            };
        }
        private object GetPeriodKey(
            int periodId)
        {
            return new
            {
                CalendarPeriodId = periodId
            };
        }
        #endregion

        #region calendar hoydays
        public async Task<IEnumerable<CalendarHoliday>> GetHolidaysAsync(
            int calendarId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            await Task.CompletedTask;

            var model = await conn.QueryAsync<CalendarHoliday>(
                SQL_Select_Star + Sql_CalendarHolidays_Table
                    + " WHERE CalendarId = @CalendarId",
                new
                {
                    CalendarId = calendarId
                });
            return await Task.FromResult(model);
        }
        public async Task<CalendarHoliday> UpdateHolidayAsync(
            CalendarHoliday model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            await Task.CompletedTask;

            var tran = conn.BeginTransaction();
            try
            {
                await this.UpdateTableAsync(
                    Sql_CalendarHolidays_Table,
                    conn, tran,
                    this.GetHolidayKey(model.CalendarId, model.Holiday),
                    this.GetHolidayData(model)
                    );
                var data = await this.InternalGetOneHolidayAsync(conn, tran, model.CalendarId, model.Holiday);
                tran.Commit();
                tran = null;
                return data;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        public async Task<CalendarHoliday> InsertHolidayAsync(
            CalendarHoliday model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            await Task.CompletedTask;

            var tran = conn.BeginTransaction();
            try
            {
                await this.InsertTableAsync(
                    Sql_CalendarHolidays_Table,
                    conn, tran,
                    this.JoinObjects(
                        this.GetHolidayKey(model.CalendarId, model.Holiday),
                        this.GetHolidayData(model)
                        )
                    );
                var data = await this.InternalGetOneHolidayAsync(conn, tran, model.CalendarId, model.Holiday);
                tran.Commit();
                tran = null;
                return data;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        public async Task DeleteHolidayAsync(
            int calendarId,
            DateTime date)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();
            await Task.CompletedTask;

            var tran = conn.BeginTransaction();
            try
            {
                // elimina tutta una corsa
                await this.DeleteTableAsync(
                    Sql_CalendarHolidays_Table,
                    conn, tran,
                    this.GetHolidayKey(calendarId, date)
                    );

                tran.Commit();
                tran = null;
            }
            finally
            {
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
        }
        private async Task<CalendarHoliday> InternalGetOneHolidayAsync(
            IDbConnection conn,
            IDbTransaction tran,
            int calendarId,
            DateTime date)
        {
            var model = await conn.QueryFirstOrDefaultAsync<CalendarHoliday>(
                SQL_Select_Star + Sql_CalendarHolidays_Table
                    + " WHERE CalendarId = @CalendarId AND Holiday = @Holiday",
                this.GetHolidayKey(calendarId, date),
                tran);
            return await Task.FromResult(model);
        }
        private object GetHolidayData(
            CalendarHoliday model)
        {
            return new
            {
                HolidayDescription = model.HolidayDescription
            };
        }
        private object GetHolidayKey(
            int calendarId,
            DateTime date)
        {
            return new
            {
                CalendarId = calendarId,
                Holiday = date
            };
        }
        #endregion
    }
}
