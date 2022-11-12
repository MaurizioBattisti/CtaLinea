using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Model
{
    public class Associate
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
        public string BsSupplierCode { get; set; }
        public string BsCustomerCode { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}
