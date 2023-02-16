using CtaLinea.Model.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface ITagRepository
    {
        Task DeleteASync(int tagId);
        Task<IEnumerable<TagForRun>> GetAllTagsAsync();
        Task<int> InsertASync(TagForRun model);
        Task UpdateASync(TagForRun model);
    }
}