using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Model
{
    public class SimpleCar
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int NrSittings { get; set; }
        public bool Active { get; set; }

        // Dati del consorziato
        public Guid AssociateId { get; set; }
        public string AssociateDescription { get; set; }
        public bool AssociateActive { get; set; }
    }
}
