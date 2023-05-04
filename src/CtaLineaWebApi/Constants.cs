using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaWebApi
{
    public static class Constants
    {
        internal const string CorsPolicyName = "MyCorsPolicy";

        // configuration section
        internal const string ConfigSection_IdentityServer = "IdentityServer";
        internal const string ConfigSection_CtaLineaDb = "CtaLineaDb";
        internal const string Configuration_MailSender = "MailSender";

        // claims type

        public const string ClaimType_AssociateId = "ctalinea.associateid";


        // importazioni
        internal const string ImportDescr_TT = "TT Service";

        // nomi delle attività
        internal const string Activity_ReloadScheduler = "reloadscheduler";
        internal const string Activity_RecalcRunDays = "calcdays";
		internal const string Activity_CleanLog = "cleanlog";
        internal const string Activity_SendMail = "sendmail";
    }
}
