using CtaLinea.Model.Costs;
using CtaLinea.Model.Runs;
using CtaLineaApp.Application.Services.Helper;
using System.Text;
using static System.Net.WebRequestMethods;

namespace CtaLineaApp.Application.Services.Costs
{
    public class CostService 
        : ICostService
    {
        private readonly IHttpService _http;

        public CostService(
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
            var url = Constants.EndPoint_CostsByAssociate;
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
    }
}
