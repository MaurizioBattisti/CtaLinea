using CtaLinea.Model.Base;
using CtaLinea.Model.Calendar;
using CtaLinea.Model.Helpers;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Base
{
    public class TagsService 
        : ITagsService
    {
        private readonly IHttpService _http;

        public TagsService(
            IHttpService htto)
        {
            _http = htto;
        }

        public async Task<IEnumerable<TagForRun>?> GetAllAsync()
        {
            var url = Constants.Endpoint_Tags;
            var data = await _http.Get<IEnumerable<TagForRun>?>(url);
            return data;
        }
        public async Task UpdateAsync(TagForRun model)
        {
            var url = string.Format(Constants.Endpoint_Tags_Single_Fmt, model.TagId);
            await this._http.Put<CheckResult>(url, model);
        }
        public async Task<int> InsertAsync(TagForRun model)
        {
            return await this._http.Post<int, CheckResult>(Constants.Endpoint_Tags, model);
        }
        public async Task DeleteAsync(int tagId)
        {
            var url = string.Format(Constants.Endpoint_Tags_Single_Fmt, tagId);
            await this._http.Delete(url);
        }
    }
}
