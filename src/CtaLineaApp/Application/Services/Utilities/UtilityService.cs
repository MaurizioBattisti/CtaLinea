using CtaLinea.Model.Checks;
using CtaLinea.Model.Costs;
using CtaLinea.Model.QueryModel;
using CtaLinea.Model.Request;
using CtaLinea.Model.Response;
using CtaLinea.Model.Runs;
using CtaLinea.Model.Utilities;
using CtaLineaApp.Application.Services.Helper;
using NPOI.SS.Formula.Functions;
using System.Text;
using static System.Net.WebRequestMethods;

namespace CtaLineaApp.Application.Services.Utilities
{
    public class UtilityService 
        : IUtilityService
    {
        private readonly IHttpService _http;

        public UtilityService(
            IHttpService http
            )
        {
            _http = http;
        }

        public async Task<IEnumerable<CarPlanningItem>> GetCarPlanningAsync(
            Guid? associateId = null,
            Guid? carId = null,
            DateTime? startDate = null,
            DateTime? endDate = null
            )
        {
            var url = Constants.EndPoint_CarPlanning;
            // crea gli arggomenti opzionali
            var args = new List<string>();
            if (associateId != null) args.Add(string.Format("{0}={1}", Constants.EndPoint_CostsByAssociate_AssociateId, associateId));
            if (carId != null) args.Add(string.Format("{0}={1}", Constants.EndPoint_CostsByAssociate_CarId, carId));
            if (startDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_StartDate, startDate));
            if (endDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_EndDAte, endDate));
            // aggiunge gli argomeni all'url
            var strArgs = string.Join("&", args);
            if (string.IsNullOrEmpty(strArgs) == false) url += "?" + strArgs;

            // esegue la chiamata al server
            var items = await _http.Get<IEnumerable<CarPlanningItem>>(url);
            return items ?? new List<CarPlanningItem>();
        }
		public async Task<IEnumerable<RunPlanningItem>> GetRunPlanningAsync(
			Guid runId,
			DateTime? startDate = null,
			DateTime? endDate = null
			)
		{
			var url = Constants.EndPoint_RunPlanning;
			// crea gli arggomenti opzionali
			var args = new List<string>();
			args.Add(string.Format("{0}={1}", Constants.EndPoint_CostsByAssociate_RunId, runId));
			if (startDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_StartDate, startDate));
			if (endDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_EndDAte, endDate));
			// aggiunge gli argomeni all'url
			var strArgs = string.Join("&", args);
			if (string.IsNullOrEmpty(strArgs) == false) url += "?" + strArgs;

			// esegue la chiamata al server
			var items = await _http.Get<IEnumerable<RunPlanningItem>>(url);
			return items ?? new List<RunPlanningItem>();
		}

		public async Task<IEnumerable<OverlappingCarItem>> GetOverlappingCarsAsync(
			Guid runCarId,
			DateTime? startDate = null,
			DateTime? endDate = null
			)
		{
			var url = Constants.EndPoint_OverlappingCars;
			// crea gli arggomenti opzionali
			var args = new List<string>();
			args.Add(string.Format("{0}={1}", Constants.EndPoint_Args_RunCarId, runCarId));
			if (startDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_StartDate, startDate));
			if (endDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_EndDAte, endDate));
			// aggiunge gli argomeni all'url
			var strArgs = string.Join("&", args);
			if (string.IsNullOrEmpty(strArgs) == false) url += "?" + strArgs;

			// esegue la chiamata al server
			var items = await _http.Get<IEnumerable<OverlappingCarItem>>(url);
			return items ?? new List<OverlappingCarItem>();
		}
        public async Task<IEnumerable<GlobalCarOverlappingItem>> GetGlobalOverlappingCarsAsync(
            DateTime? startDate = null,
            DateTime? endDate = null
            )
        {
            var url = Constants.EndPoint_RunGlobalOverlappingCars;
            // crea gli arggomenti opzionali
            var args = new List<string>();
            if (startDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_StartDate, startDate));
            if (endDate != null) args.Add(string.Format("{0}={1:yyyy-MM-dd}", Constants.EndPoint_CostsByAssociate_EndDAte, endDate));
            // aggiunge gli argomeni all'url
            var strArgs = string.Join("&", args);
            if (string.IsNullOrEmpty(strArgs) == false) url += "?" + strArgs;

            // esegue la chiamata al server
            var items = await _http.Get<IEnumerable<GlobalCarOverlappingItem>>(url);
            return items ?? new List<GlobalCarOverlappingItem>();
        }

        

        public async Task<IEnumerable<Guid>?> GetCarsForDiscontinuationAsunc (
			Guid associateId,
			DateTime? refDate)
		{
			var url = Constants.Endpoint_Utility_CarForDiscontinuation + "?" + Constants.EndPoint_Args_AssociateId + "=" + associateId.ToString();
			if (refDate != null)
			{
				url += "&" + Constants.EndPoint_Args_RefDate + string.Format("=" + Constants.DateArgs_Format, refDate.Value);
			}
            var items = await _http.Get<IEnumerable<Guid>>(url);
			return items;
        }
        public async Task ReplaceCarsAsync(
            IDictionary <Guid, Guid> carMap,
            DateTime? refDate)
        {
			var url = Constants.Endpoint_Utility_ReplaceCars;
            if (refDate != null)
            {
                url += "?" + Constants.EndPoint_Args_RefDate + string.Format("=" + Constants.DateArgs_Format, refDate.Value);
            }
            await _http.Post(url, carMap);
        }

		public async Task<IEnumerable<RunIncongruenceModel>?> GetRunIncongruenceAsync (
            RinIncongruenceRequest request)
		{
            var url = Constants.EndPoint_RunIncongruence;
            var items = await _http.Post<IEnumerable<RunIncongruenceModel>, string>(url, request);
            return items;
        }

        public async Task<OperationResponse?> AddElastibusDaysAsync(
            AddElastibusDaysRequest request)
        {
            var url = Constants.Endpoint_Run_AddElastibusDays;
            var result = await _http.Post<OperationResponse, OperationResponse>(url, request);
            return result;
        }

        public async Task<IEnumerable<string>?> GetSimulationNamesAsync ()
        {
            var url = Constants.Endpoint_Simulations;
            var items = await _http.Get<IEnumerable<string>>(url);
            return items;
        }
        public async Task DeleteSimulationAsync (
            string simulationName)
        {
            var url = string.Format(Constants.Endpoint_Simulations_One_fmt, simulationName);
            await this._http.Delete(url);
        }

        public async Task<SimulationStatusResponse?> GetSimulationStatusAsync(
            SimulationRequest request)
        {
            var url = Constants.Endpoint_Simulations_Status;
            var status = await _http.Post<SimulationStatusResponse, string>(url, request);
            return status;
        }
        public async Task SimulationApplyChangesASync(
            ApplyChangesSimulationRequest request)
        {
            var url = Constants.Endpoint_Simulations_Apply;
            await _http.Post(url, request);
        }
        public async Task SimulationKmTotalApplyChangesASync(
            ApplyChangeToSimRequest<KmToMatchCarRequest> request)
        {
            var url = Constants.Endpoint_Simulations_KmTotal_Apply;
            await _http.Post(url, request);
        }

    }
}
