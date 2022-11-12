using CtaLineaApp.Application.Model;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Import
{
    public class ImportDataService
    {
        private readonly IHttpService _httpService;

        public ImportDataService (
            IHttpService httpService
            )
        {
            _httpService = httpService;
        }

        public async Task<ImportQueryItem?> GetPendingImportAsunc (
            string importTypeId)
        {
            string endpoint = string.Format("api/importlogs/pending/{0}",
                importTypeId);

            var result = await  _httpService.Get<ImportQueryItem>(endpoint);
            return result;
        }
    }
}
