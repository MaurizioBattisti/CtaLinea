using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.QueryModel;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface ICategoriesQueries
    {
        //Categorie
        Task<QueryItemList<CategoryQueryItem>> GetCategoryListAsync(
            IFilteringContext filterContext);
        Task<CategoryQueryItem> GetOneCategoryAsync(
            string id);
        
        // Sottocategorie
        Task<QueryItemList<SubCategoryQueryItem>> GetSubCategoryListAsync(
            IFilteringContext filterContext);
        Task<QueryItemList<SubCategoryQueryItem>> GetCategorySubCategoryListAsync(
            IFilteringContext filterContext,
            string categoryId);
        Task<SubCategoryQueryItem> GetOneSubCategoryAsync(
    string id);

    }
}