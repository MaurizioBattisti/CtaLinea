using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.External
{
    public class Car
    {
        public Guid Id { get; set; }
        public Guid AssociateId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int NrSittings { get; set; }
        public string RegNumber { get; set; } = string.Empty;
        public string? BsCarId { get; set; }
        public string? ChassisNumber { get; set; }
        public DateTime? FirstRegistration { get; set; }
        public DateTime? DiscontinuationDate { get; set; }
        public bool PrimaryCar { get; set; }
        public bool SpareCar { get; set; }
        public bool Active { get; set; }

        public override bool Equals(object o)
        {
            var other = o as Car;

            return other?.Id == Id;
        }

        public override string ToString()
        {
            return this.Description;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
