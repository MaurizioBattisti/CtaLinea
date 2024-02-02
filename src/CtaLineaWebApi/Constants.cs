using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaWebApi
{
    public static class Constants
    {
        // HTTP header da inviare
        internal const string RequestHeader_PeriodStartDate = "x-zz-start";
        internal const string RequestHeader_PeriodEndDate = "x-zz-end";
        internal const string RequestHeader_ContractId = "x-zz-contract";

        internal const string CorsPolicyName = "MyCorsPolicy";

        // configuration section
        internal const string ConfigSection_IdentityServer = "IdentityServer";
        internal const string ConfigSection_CtaLineaDb = "CtaLineaDb";
        internal const string Configuration_MailSender = "MailSender";
        internal const string Configuration_Importer = "Importer";
        internal const string ConfigSection_TaskScheduler = "TaskScheduler";

		// claims type

		public const string ClaimType_AssociateId = "ctalinea.associateid";


        // importazioni
        internal const string ImportDescr_TT = "TT Service";

        // nomi delle attività
        internal const string Activity_ReloadScheduler = "reloadscheduler";
        internal const string Activity_RecalcRunDays = "calcdays";
		internal const string Activity_CleanLog = "cleanlog";
        internal const string Activity_SendMail = "sendmail";
        internal const string Activity_RecalcCosts = "recalcosts";
        internal const string Activity_ImportAssociate = "i_asso";
        internal const string Activity_ImportPoints = "i_points";
        internal const string Activity_ExportElastibus = "exp_elastibus";
        internal const string Activity_ExporTT = "exp_tt";
        internal const string Activity_ExecuteSql= "exec";

        // Policy
        internal const string Policy_ChangePAssword = "ChangePAssword";
		
        internal const string Policy_Tasks = "Tasks";
		internal const string Policy_Users = "Users";

		internal const string Policy_ViewData = "viewData";
        internal const string Policy_ManageData = "ManageData";
		internal const string Policy_Planning= "Planning";
		internal const string Policy_Costs = "Costs";
		internal const string Policy_RunView = "RunView";
		internal const string Policy_RunEdit = "RunEdit";

        internal const string BudgetType_LastCalc = "LAST CALC";
    }
}
