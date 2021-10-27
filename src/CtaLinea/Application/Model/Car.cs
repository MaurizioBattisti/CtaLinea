using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLinea.Application.Model
{
    public class Car
    {
        public Guid Id { get; set; }

        public string Description { get; set; }
        public string RegNumber { get; set; }

        public int NrSittings { get; set; }

        public string BsCarId { get; set; }

        public string ChassisNumber { get; set; }
        public DateTime? FirstRegistration { get; set; }
        public DateTime? DiscontinuationDate { get; set; }

        public bool PrimaryCar { get; set; }
        public bool SpareCar { get; set; }

        public bool Active { get; set; }

        // Dati del consorziato
        public Guid AssociateId { get; set; }
        public string AssociateDescription { get; set; }
        public bool AssociateActive { get; set; }
    }
}
