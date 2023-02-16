using CtaLinea.Model.Base;

namespace CtaLineaApp.Application.Services.Base
{
    public interface ITagsService
    {
        Task DeleteAsync(int tagId);
        Task<IEnumerable<TagForRun>?> GetAllAsync();
        Task<int> InsertAsync(TagForRun model);
        Task UpdateAsync(TagForRun model);
    }
}