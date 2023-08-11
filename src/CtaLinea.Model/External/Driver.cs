using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.External
{
    public class Driver
    {
		public Guid DriverId { get; set; }
		public Guid AssociateId { get; set; }

		public string LastName { get; set; } = string.Empty;
		public string FirstName { get; set; } = string.Empty;

		public string? BsDriverId { get; set; }
		public string LicenseNumber { get; set; } = string.Empty;
		public string? LicenceCategory { get; set; }

		public DateTime? DismissionDate { get; set; }

		public bool Active { get; set; }

		public override bool Equals(object o)
		{
			var other = o as Driver;

			return other?.DriverId == this.DriverId;
		}

		public override string ToString()
		{
			return this.LastName + " " + this.FirstName;
		}

		public override int GetHashCode()
		{
			return this.DriverId.GetHashCode();
		}
	}
}
