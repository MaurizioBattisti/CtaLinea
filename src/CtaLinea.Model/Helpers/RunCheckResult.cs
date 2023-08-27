using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Helpers
{
    public class RunCheckResult
        : CheckResult
    {
        // costanti con i valori delle categorie
        public const string Category_Run = "RUN";
        public const string Category_RunVariation = "VARIATION";
        public const string Category_Node = "NODE";
        public const string Category_Period = "PERIOD";
        public const string Category_Car = "CAR";
        public const string Category_CarCost = "COST";
        public const string Category_CarReplacement = "REPLACEMENT";
        public const string Category_Suspension = "SUSPENSION";
        public const string Category_ElbDays = "ELB_DAYS";
    }
}