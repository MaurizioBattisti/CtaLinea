using CtaLinea.Model.External;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("d")]
    public class DriverQueryItem
	{
		[SqlField("DriverId", SortPosition = 2)]
		public Guid Id { get; set; }

		[SqlField(FullText = true)]
		public string LastName { get; set; } = string.Empty;
		[SqlField(FullText = true)]
		public string FirstName { get; set; } = string.Empty;

		[SqlField(SortPosition = 1, FullText = true)]
		public string CompleteName { get; set;} = string.Empty;


		[SqlField(FullText = true)]
		public string? BsDriverId { get; set; }
		[SqlField(FullText = true)]
		public string LicenseNumber { get; set; } = string.Empty;
		[SqlField(FullText = true)]
		public string? LicenceCategory { get; set; }

		public DateTime? DismissionDate { get; set; }

		public bool Active { get; set; }

		// Dati del consorziato
		[SqlAlias("a")]
		public Guid AssociateId { get; set; }

		[SqlAlias("a")]
		[SqlField("Description", FullText = true)]
		public string? AssociateDescription { get; set; }
		[SqlAlias("a")]
		[SqlField("Active")]
		public bool AssociateActive { get; set; }

		public override bool Equals(object o)
		{
			var other = o as DriverQueryItem;

			return other?.Id == this.Id;
		}

		public override string ToString()
		{
			return this.LastName + " " + this.FirstName;
		}

		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}
	}
}
