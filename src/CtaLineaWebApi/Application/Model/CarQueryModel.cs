using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Model
{
    public class CarQueryModel
    {
        public Guid Id { get; set; }
        public string Associate { get; set; }
        public Guid AssociateId { get; set; }
        public int StartNumber { get; set; }
        public bool Active { get; set; }

        public int Sittings { get; set; }
    }
}
