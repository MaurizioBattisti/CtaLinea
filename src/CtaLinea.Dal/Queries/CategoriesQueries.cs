using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.QueryModel;
using ZzSoft.QueryHelper;


namespace ZzSoft.CtaLinea.Dal.Queries
{
    public class CategoriesQueries 
        : ICategoriesQueries
    {
        private const string CategorySql_Table = "dbo.SheetCategories c";
        private const string SubCategorySql_Table = "dbo.SheetSubCategories s INNER JOIN dbo.SheetCategories c ON s.CategoryId = c.CategoryId";

        private readonly CtaDbContext _context;

        public CategoriesQueries(
            CtaDbContext context)
        {
            this._context = context;
        }

        //  Categorie
        public async Task<QueryItemList<CategoryQueryItem>> GetCategoryListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<CategoryQueryItem>(
                CategorySql_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<CategoryQueryItem> GetOneCategoryAsync(
            string id)
        {
            var queryDef = new QueryDefinition<CategoryQueryItem>(
                CategorySql_Table,
                null,
                "c.CategoryId = @CategoryId",
                new { CategoryId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }

        // sottocategorie
        public async Task<QueryItemList<SubCategoryQueryItem>> GetSubCategoryListAsync(
            IFilteringContext filterContext)
        {
            var queryDef = new QueryDefinition<SubCategoryQueryItem>(
                SubCategorySql_Table,
                filterContext);

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<QueryItemList<SubCategoryQueryItem>> GetCategorySubCategoryListAsync(
            IFilteringContext filterContext,
            string categoryId)
        {
            var queryDef = new QueryDefinition<SubCategoryQueryItem>(
                SubCategorySql_Table,
                filterContext,
                "s.CategoryId = @CategoryId",
                new { CategoryId = categoryId });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryListAsync(
                queryDef)
                .ConfigureAwait(false);
        }
        public async Task<SubCategoryQueryItem> GetOneSubCategoryAsync(
            string id)
        {
            var queryDef = new QueryDefinition<SubCategoryQueryItem>(
                SubCategorySql_Table,
                null,
                "s.SubCategoryId = @SubCategoryId",
                new { SubCategoryId = id });

            IDbConnection conn = this._context.Database.GetDbConnection();
            return await conn.QueryOneAsync(
                queryDef)
                .ConfigureAwait(false);
        }
    }
}
