using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaApp
{
    internal class Constants
    {
        // Titolo applicazione
        public const string App_Title =  "CTA Linee";


        internal const string LoalStorageKey_User = "user";


        // configuration
        internal const string Config_CtaLineaApi = "CtaLineaApi";


        // import status
        internal const string ImportStatus_Error = "ERROR";
        internal const string ImportStatus_Aborted = "ABORTED";
        internal const string ImportStatus_Progress = "PROGRESS";
        internal const string ImportStatus_Completed = "COMPLETED";

        // scopes
        internal const string Scope_App = "app";
        internal const string Scope_Http_Api = "api";

        // headers del response
        internal const string ResponseHeader_TotalRows = "X-Total-Count";

        // import type description
        internal const string ImportDescr_TT = "TT Service";



        // endpoints
        internal const string Endpoint_Runs = "/api/runs";
        internal const string Endpoint_RunList = Endpoint_Runs;
        internal const string Endpoint_RunList_Adv = Endpoint_Runs + "/advanced";
        internal const string Endpoint_OneRun_Frm= Endpoint_Runs  + "/{0}";
		internal const string Endpoint_Run_Detail_Fmt = Endpoint_OneRun_Frm + "/detail";
		internal const string Endpoint_RunVariationList_Fmt = Endpoint_OneRun_Frm  + "/variations";
		internal const string Endpoint_Run_Tags_Fmt = Endpoint_OneRun_Frm + "/tags";

		// costi
		internal const string EndPoint_Costs = "api/costs";
        internal const string EndPoint_CostsByAssociate = EndPoint_Costs + "/byassociate";
        // utility
        internal const string EndPoint_Utilitys = "api/utility";
        internal const string EndPoint_CarPlanning = EndPoint_Utilitys + "/carplanning";
		internal const string EndPoint_RunPlanning = EndPoint_Utilitys + "/runplanning";
        internal const string EndPoint_OverlappingCars = EndPoint_Utilitys + "/overlappingcars";
        
        // argomenti
        internal const string EndPoint_CostsByAssociate_ContractId = "contractId";
        internal const string EndPoint_CostsByAssociate_StartDate= "startDate";
        internal const string EndPoint_CostsByAssociate_EndDAte = "endDate";
        internal const string EndPoint_CostsByAssociate_AssociateId = "associateId";
        internal const string EndPoint_CostsByAssociate_CarId = "carId";
        internal const string EndPoint_CostsByAssociate_RunId = "runId";

		internal const string EndPoint_Args_RunCarId = "runCarId";

		// Calenari
		internal const string Endpoint_Calendars = "/api/calendars";
        internal const string Endpoint_Calendar_One_Fmr = Endpoint_Calendars + "/{0}";
        internal const string Endpoint_Calendar_SingleOne_Fmr = Endpoint_Calendar_One_Fmr + "/simple";
        internal const string Endpoint_Calendar_Periods_Fmr = Endpoint_Calendar_One_Fmr + "/periods";
        internal const string Endpoint_Calendar_Holidays_Fmr = Endpoint_Calendar_One_Fmr + "/holidays";
        internal const string Endpoint_Calendar_Holidays_Single_Fmr = Endpoint_Calendar_Holidays_Fmr + "/{1:yyyy-MM-dd}";
        internal const string Endpoint_CalendarPeriods = Endpoint_Calendars + "/periods";
        internal const string Endpoint_CalendarPeriods_Single_Fmt = Endpoint_CalendarPeriods + "/{0}";
        internal const string Endpoint_Calendar_Days = Endpoint_Calendar_One_Fmr + "/days?startDate={1:yyyy-MM-dd}&endDate={2:yyyy-MM-dd}";

        // TAgs
        internal const string Endpoint_Tags = "/api/tags";
        internal const string Endpoint_Tags_Single_Fmt = Endpoint_Tags + "/{0}";

        // contratti
        internal const string Endpoint_Contracts = "/api/contracts";
		internal const string Endpoint_ContractsOperationalPeriods = Endpoint_Contracts+ "/periods";
        internal const string Endpoint_OneContract_Fmt = Endpoint_Contracts + "/{0}";
        
        // ditte
        internal const string Endpoint_Associates = "/api/associates";
        internal const string Endpoint_Cars = "/api/cars";

		// punti di raccolta
		internal const string Endpoint_CollectionPoints = "/api/collectionpoints";

		// utility
		internal const string Endpoint_Utility = "/api/utility";
		internal const string Endpoint_Utility_CarForDiscontinuation = Endpoint_Utility + "/carsfordiscontinuation";
		internal const string Endpoint_Utility_ReplaceCars = Endpoint_Utility+ "/replacecars";


		internal const string Endpoint_Drivers = "/api/drivers";
        internal const string Endpoint_PendingImport = "/api/importlogs/pending/{0}";
        internal const string Endpoint_ImportLog = "/api/importlogs";
        internal const string Endpoint_ImportLog_Single = "/api/importlogs/{0}";
        internal const string Endpoint_ImportLog_Details = "/api/importlogs/{0}/details";
        
        internal const string Endpoint_TtServies = "/api/ttservices";

        internal const string Endpoint_TT_Import = "/api/utility/ttservices";

        internal const string Endpoint_Info = "/api/info";
        internal const string Endpoint_Info_Versions = Endpoint_Info +"/versions";

        // internal const int Default_PageSize = 20;
    }
}