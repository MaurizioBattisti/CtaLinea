using CtaLinea.Model.Base;
using CtaLinea.Model.Runs;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public class TagRepository
        : RepositoryBase
        , ITagRepository
    {
        private const string SQL_TagsTable = "[dbo].[Tags]";

        private readonly CtaDbContext _context;

        private readonly ILogger _logger;
        public TagRepository(
            CtaDbContext context,
            ILogger<TagRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TagForRun>> GetAllTagsAsync()
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            return await conn.QueryAsync<TagForRun>(
                SQL_Select_Star + SQL_TagsTable);
        }

        public async Task<int> InsertASync(
            TagForRun model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            await this.InsertTableAsync(
                SQL_TagsTable,
                conn, null,
                this.GetTagData(model));
            return await this.GetIdentityAsync(conn);
        }
        public async Task UpdateASync(
            TagForRun model)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            await this.UpdateTableAsync(
                SQL_TagsTable,
                conn, null,
                this.GetTagKey(model.TagId),
                this.GetTagData(model)
                );
        }
        public async Task DeleteASync(
            int tagId)
        {
            using IDbConnection conn = this._context.Database.GetDbConnection();
            conn.Open();

            await this.DeleteTableAsync(
                SQL_TagsTable,
                conn, null,
                this.GetTagKey(tagId)
                );
        }

        private object GetTagData(
            TagForRun model)
        {
            return new
            {
                model.Ordinal,
                model.TagName,
                model.BgColor,
                model.Color
            };
        }
        private object GetTagKey(int id)
        {
            return new
            {
                TagId = id
            };
        }
    }
}
