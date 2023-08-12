using CtaLinea.Model.Costs;
using CtaLinea.Model.Runs;
using CtaLineaApp.Application.Services.Helper;
using System;
using System.Text;
using static System.Net.WebRequestMethods;

namespace CtaLineaApp.Application.Services.Costs
{
    public class CostServic
        : ICostService
    {
        private readonly IHttpService _http;

        public CostServic(
            IHttpService http
            )
        {
            _http = http;
        }
        public async Task<IEnumerable<CostsByAssociate>> GetCostsByAssociateAsync(
            int? contractId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            Guid? associateId = null,
            Guid? carId = null,
            Guid? runId = null
            )
        {
            var url = Constants.EndPoint_CostsByAssociate_GET;
            // crea gli arggomenti opzionali
            var args = new List<string>();
            if (contractId != null) args.Add(string.Format("{0}={1}", Constants.EndPoint_CostsByAssociate_ContractId, contractId));
            if (startDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_StartDate, startDate));
            if (endDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_EndDAte, endDate));
            if (associateId != null) args.Add(string.Format("{0}={1}", Constants.EndPoint_CostsByAssociate_AssociateId, associateId));
            if (carId != null) args.Add(string.Format("{0}={1}", Constants.EndPoint_CostsByAssociate_CarId, carId));
            if (runId != null) args.Add(string.Format("{0}={1}", Constants.EndPoint_CostsByAssociate_RunId, runId));
            // aggiunge gli argomeni all'url
            var strArgs = string.Join("&", args);
            if (string.IsNullOrEmpty(strArgs) == false) url += "?" + strArgs;

            // esegue la chiamata al server
            var items = await _http.Get<IEnumerable<CostsByAssociate>>(url);
            return items;
        }

        public async Task<IEnumerable<CostByAssociateItem>?> GetCostsByAssociateASync(
            CalcCostsRequest request)
        {
            var url = Constants.EndPoint_CostsByAssociate;
            var items = await _http.Post<IEnumerable<CostByAssociateItem>, string>(url, request);
            return items;
        }
		public async Task<IEnumerable<CostByAssociateByRunItem>?> GetCostsByAssociateByRunASync(
			CalcCostsRequest request)
		{
			var url = Constants.EndPoint_CostsByAssociateByRun;
			var items = await _http.Post<IEnumerable<CostByAssociateByRunItem>, string>(url, request);
			return items;
		}
		public async Task<IEnumerable<CostByRunItem>?> GetCostsByRunASync(
            CalcCostsRequest request)
        {
            var url = Constants.EndPoint_CostsByRun;
            var items = await _http.Post<IEnumerable<CostByRunItem>, string>(url, request);
            return items;
        }
    }
}
