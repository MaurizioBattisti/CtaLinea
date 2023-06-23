using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Utilities
{
    public class RinIncongruenceRequest
    {
        public Guid? RunId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IEnumerable<int>? WhatIncongruence { get;set; }
    }
}
