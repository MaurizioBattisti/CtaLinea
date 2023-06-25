using CtaLinea.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class SetSuspensionRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SuspensionTypeId { get; set; }
        public SuspensionType? SuspensionTypeData { get; set; }
        public string? SuspensionNote { get; set; }
    }
}
