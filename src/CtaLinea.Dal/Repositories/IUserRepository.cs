using CtaLinea.Model.Base;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Model;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> GetUserAsync(string userName);
        Task<UserEntity> InsertUSerAsync(
			UserEntity user);
        Task<UserEntity> UpdateUserAsync(
            UserEntity user);
        Task DeleteUserAsync(
            string userName);
        Task<UserEntity> SetUserPasswordAsync(
            string userName,
            string passwordHash,
            bool setMustChange = false);
    }
}