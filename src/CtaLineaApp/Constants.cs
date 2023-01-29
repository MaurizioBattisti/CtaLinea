using System;
using System.Collections.Generic;
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
        internal const string Endpoint_OneRun_Frm= Endpoint_Runs  + "/{0}";
        internal const string Endpoint_RunVariationList_Fmt = Endpoint_OneRun_Frm  + "/variations";

        // costi
        internal const string EndPoint_Costs = "api/costs";
        internal const string EndPoint_CostsByAssociate = EndPoint_Costs + "/byassociate";
        // utility
        internal const string EndPoint_Utilitys = "api/utility";
        internal const string EndPoint_CarPlanning = EndPoint_Utilitys + "/carplanning";

        // argomenti
        internal const string EndPoint_CostsByAssociate_ContractId = "contractId";
        internal const string EndPoint_CostsByAssociate_StartDate= "startDate";
        internal const string EndPoint_CostsByAssociate_EndDAte = "endDate";
        internal const string EndPoint_CostsByAssociate_AssociateId = "associateId";
        internal const string EndPoint_CostsByAssociate_CarId = "carId";
        internal const string EndPoint_CostsByAssociate_RunId = "runId";


        internal const string Endpoint_Calendars = "/api/calendars";
        internal const string Endpoint_Calendar_One_Fmr = Endpoint_Calendars + "/{0}";
        internal const string Endpoint_Calendar_SingleOne_Fmr = Endpoint_Calendar_One_Fmr + "/single";
        internal const string Endpoint_Calendar_Periods_Fmr = Endpoint_Calendar_One_Fmr + "/periods";
        internal const string Endpoint_Calendar_Holidays_Fmr = Endpoint_Calendar_One_Fmr + "/holidays";
        internal const string Endpoint_Contracts = "/api/contracts";        
        internal const string Endpoint_Associates = "/api/associates";
        internal const string Endpoint_Cars = "/api/cars";

        internal const string Endpoint_Drivers = "/api/drivers";
        internal const string Endpoint_PendingImport = "/api/importlogs/pending/{0}";
        internal const string Endpoint_ImportLog = "/api/importlogs";
        internal const string Endpoint_ImportLog_Single = "/api/importlogs/{0}";
        internal const string Endpoint_ImportLog_Details = "/api/importlogs/{0}/details";
        
        internal const string Endpoint_TtServies = "/api/ttservices";

        internal const string Endpoint_TT_Import = "/api/utility/ttservices";

        // internal const int Default_PageSize = 20;
    }
}