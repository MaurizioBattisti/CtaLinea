using CtaLinea.Model.Contab;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.ModelServices;
using CtaLinea.Model.QueryModel;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Contab
{
    public class ForfaitService 
        : IForfaitService
    {
        private readonly IHttpService _http;

        public ForfaitService(
            IHttpService http
            )
        {
            _http = http;
        }

        public async Task<ForfaitQueryItem?> GetOneAsync(int id)
        {
            var url = string.Format(Constants.Endpoint_Forfait_Single_Fmt, id);
            var data = await this._http.Get<ForfaitQueryItem>(url);
            return data;
        }

        public async Task<int?> InsertOneAsync(MultiRunForfait model)
        {
            return await this._http.Post<int, CheckResult>(Constants.Endpoint_Forfaits, model);
        }
        public async Task UpdateOneAsync(int forfaitOd, MultiRunForfait model)
        {
            var url = string.Format(Constants.Endpoint_Forfait_Single_Fmt, forfaitOd);
            await this._http.Put<CheckResult>(url, model);
        }
        public async Task DeleteOneAsync(int forfaitOd)
        {
            var url = string.Format(Constants.Endpoint_Forfait_Single_Fmt, forfaitOd);
            await this._http.Delete(url);
        }
        public async Task SetDetails(int? forfait, IEnumerable<Guid>? runIds)
        {
            var model = new MultiRunForfaitOperation()
            {
                ForfaitId = forfait,
                RunIds = runIds
            };
            await this._http.Post<CheckResult>(Constants.Endpoint_Forfait_SetRuns, model);
        }
    }
}
