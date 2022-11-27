using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Base
{
    public class Contract
    {
        public int ContractId { get; set; }
        public string? ContractName { get; set; }
        public string? ContractDescription { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
