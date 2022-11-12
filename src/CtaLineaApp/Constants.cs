using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaApp
{
    internal class Constants
    {
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
        internal const string Endpoint_Associates = "/api/associates";
        internal const string Endpoint_Cars = "/api/cars";
        internal const string Endpoint_Drivers = "/api/drivers";
        internal const string Endpoint_PendingImport = "/api/importlogs/pending/{0}";
        internal const string Endpoint_ImportLog = "/api/importlogs";
        internal const string Endpoint_ImportLog_Single = "/api/importlogs/{0}";
        internal const string Endpoint_ImportLog_Details = "/api/importlogs/{0}/details";
        
        internal const string Endpoint_TtServies = "/api/ttservices";

        internal const string Endpoint_TT_Import = "/api/utility/ttservices";

        internal const int Default_PageSize = 20;
    }
}
