using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserAsync(string userName);
        Task<UserEntity> UpdateUserAsync(UserEntity user);
    }
}