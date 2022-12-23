using CtaLinea.Model.Runs;
using CtaLineaApp.Application.Services.Helper;

namespace CtaLineaApp.Application.Services.Run
{
    public class RunRepository 
        : IRunRepository
    {
        private readonly IHttpService _http;

        public RunRepository(
            IHttpService http
            )
        {
            _http = http;
        }

        public async Task<RunItem?> GetOneAsync(
            Guid runId)
        {
            string url = string.Format(Constants.Endpoint_OneRun_Frm, runId);
            var run = await _http.Get<RunItem>(url);
            return run;
        }
    }
}
