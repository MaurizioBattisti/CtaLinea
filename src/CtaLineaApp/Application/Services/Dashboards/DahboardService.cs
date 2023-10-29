using CtaLinea.Model.Checks;
using CtaLinea.Model.Costs;
using CtaLinea.Model.Dashboards;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Reports;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using CtaLinea.Model.Runs;
using CtaLinea.Model.Utilities;
using CtaLineaApp.Application.Services.Helper;
using NPOI.SS.Formula.Functions;
using System.Text;
using static System.Net.WebRequestMethods;

namespace CtaLineaApp.Application.Services.Dashboards
{
    public class DahboardService 
        : IDahboardService
    {
        private readonly IHttpService _http;

        public DahboardService(
            IHttpService http
            )
        {
            _http = http;
        }

        public async Task<CarCountResult?> GetCarCountAsync(
            CarCountRequest request)
        {
            var url = Constants.Endpoint_Dashboard_CarCount;
            var result = await _http.Post<CarCountResult, string>(url, request);
            return result;
        }
		public async Task<IEnumerable<KmDataItem>?> GetKmAsync(
			KmDataItemRequest request)
		{
			var url = Constants.Endpoint_Dashboard_Km;
			var result = await _http.Post<IEnumerable<KmDataItem>, string>(url, request);
			return result;
		}
		public async Task<IEnumerable<CostDataItem>?> GetCostsAsync(
			CostDataItemRequest request)
		{
			var url = Constants.Endpoint_Dashboard_Costs;
			var result = await _http.Post<IEnumerable<CostDataItem>, string>(url, request);
			return result;
		}
	}
}
