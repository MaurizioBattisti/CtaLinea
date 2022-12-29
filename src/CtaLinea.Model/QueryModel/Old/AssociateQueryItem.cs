using ZzSoft.QueryHelper;

namespace CtaLinea.QueryModel
{
    [SqlAlias("a")]
    public class AssociateQueryItem
    {
        [SqlField("AssociateId")]
        public Guid Id { get; set; }

        [SqlField(SortPosition = 0, FullText = true)]
        public string Description { get; set; } = String.Empty;
        [SqlField(FullText = true)]
        public string? BsSupplierCode { get; set; }
        [SqlField(FullText = true)]
        public string? BsCustomerCode { get; set; }
        [SqlField(FullText = true)]
        public string? Email { get; set; }
        public bool Active { get; set; }

        /*
        public override bool Equals(object? o)
        {
            var other = o as CarQueryItem;

            return (other == null ? false : other.Id == Id);
        }

        public override string ToString()
        {
            return this.Description;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        */
    }
}
