using CtaLinea.Model.Contab;
using CtaLinea.Model.QueryModel;

namespace CtaLineaApp.Application.Services.Contab
{
    public interface IForfaitService
    {
        Task<ForfaitQueryItem?> GetOneAsync(int id);
        Task DeleteOneAsync(int forfaitOd);
        Task<int> InsertOneAsync(MultiRunForfait model);
        Task SetDetails(int? forfait, IEnumerable<Guid>? runIds);
        Task UpdateOneAsync(int forfaitOd, MultiRunForfait model);
    }
}