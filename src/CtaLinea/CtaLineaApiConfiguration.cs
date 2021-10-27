using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLinea
{
    public class CtaLineaApiConfiguration
    {
        public string BaseAddress { get; set; }
        public string[] Scopes { get; set; }
        public int DefaultPageSize { get; set; } = 20;
    }
}
