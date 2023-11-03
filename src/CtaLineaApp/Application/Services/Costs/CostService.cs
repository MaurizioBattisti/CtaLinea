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

        public async Task DeleteBudgetAsync (int budgetId)
        {
            var url = string.Format(Constants.Endpoint_Budget_One_Fmt, budgetId);
            await _http.Delete<string>(url);
	    }

		public async Task<IEnumerable<CostByAssociateItem>?> GetCostsByAssociateASync(
            int budgetId,
			CalcCostsRequest? request)
		{
            if (request == null) request = new CalcCostsRequest();
			var url = string.Format(Constants.Endpoint_Budget_One_Detail_Fmt, budgetId);
			var items = await _http.Post<IEnumerable<CostByAssociateItem>, string>(url, request);
			return items;
		}

		#region calcola i totlai
		public CostByAssociateItem ComputeTotalsByCar(IEnumerable<CostByAssociateItem> costs)
        {
            var total = new CostByAssociateItem()
            {
                Km = 0,
                KmExtra = 0,
                DayCost = 0,
                CostKm = 0,
                CostKmExtra = 0,
                DayIntegration = 0,
                DayForfait = 0,
                DayMultiRunForfait = 0
            };
            foreach (var c in costs)
            {
                total.Km += c.Km;
                total.KmExtra += c.KmExtra;
                total.DayCost += c.DayCost;
                total.CostKm += c.CostKm;
                total.CostKmExtra += c.CostKmExtra;

                total.DayIntegration += c.DayIntegration;
                total.DayForfait += c.DayForfait;
                total.DayMultiRunForfait += c.DayMultiRunForfait;
            }

            return total;
        }
        public CostByAssociateByRunItem ComputeTotalsByRun(IEnumerable<CostByAssociateByRunItem> costs)
        {
            var total = new CostByAssociateByRunItem()
            {
                Km = 0,
                KmExtra = 0,
                DayCost = 0,
                CostKm = 0,
                CostKmExtra = 0,
                DayIntegration = 0,
                DayForfait = 0,
                DayMultiRunForfait = 0
            };
            foreach (var c in costs)
            {
                total.Km += c.Km;
                total.KmExtra += c.KmExtra;
                total.DayCost += c.DayCost;
                total.CostKm += c.CostKm;
                total.CostKmExtra += c.CostKmExtra;

                total.DayIntegration += c.DayIntegration;
                total.DayForfait += c.DayForfait;
                total.DayMultiRunForfait += c.DayMultiRunForfait;
            }

            return total;
        }
        #endregion
    }
}
