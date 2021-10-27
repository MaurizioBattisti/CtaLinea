using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Configuration
{
    public class IdentityServerConfiguration
    {
        public string Authority { get; set; }
        public string Audience { get; set; }
    }
}
