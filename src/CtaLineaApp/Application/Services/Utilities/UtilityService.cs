using CtaLinea.Model.Checks;
using CtaLinea.Model.Costs;
using CtaLinea.Model.Runs;
using CtaLinea.Model.Utilities;
using CtaLineaApp.Application.Services.Helper;
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
	}
}
