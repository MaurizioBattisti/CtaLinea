using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("c")]
    public class CarQueryItem
    {
        [SqlField("CarId")]
        public Guid Id { get; set; }

        [SqlField(FullText = true)]
        public string Description { get; set; } = string.Empty;
        [SqlField(FullText = true)]
        public string RegNumber { get; set; } = string.Empty;

        public int NrSittings { get; set; }

        [SqlField(FullText = true)]
        public string? BsCarId { get; set; }

        [SqlField(FullText = true)]
        public string? ChassisNumber { get; set; }
        public DateTime? FirstRegistration { get; set; }
        public DateTime? DiscontinuationDate { get; set; }

        public bool PrimaryCar { get; set; }
        public bool SpareCar { get; set; }

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

        public override bool Equals(object? o)
        {
            var other = o as CarQueryItem;

            return other == null ? false : other.Id == Id;
        }

        public override string ToString()
        {
            return Description;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
