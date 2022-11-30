using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.External
{
    public class Associate
    {
        public Guid AssociateId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? BsSupplierCode { get; set; }
        public string? BsCustomerCode { get; set; }
        public bool Active { get; set; }
        public string? Email { get; set; }
}
}
