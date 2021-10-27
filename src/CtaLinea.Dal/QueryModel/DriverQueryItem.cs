using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("d")]
    public class DriverQueryItem
    {
        [SqlField("DriverId")]
        public Guid Id { get; set; }

        [SqlField(SortPosition = 0, FullText = true)]
        public string LastName { get; set; }
        [SqlField(SortPosition = 1, FullText = true)]
        public string FirstName { get; set; }

        [SqlField(FullText = true)]
        public string BsDriverId { get; set; }
        [SqlField(FullText = true)]
        public string LicenseNumber { get; set; }
        [SqlField(FullText = true)]
        public string LicenceCategory { get; set; }

        public DateTime? DismissionDate { get; set; }

        public bool Active { get; set; }

        // Dati del consorziato
        [SqlAlias("a")]
        public Guid AssociateId { get; set; }
        [SqlAlias("a")]
        [SqlField("Description", FullText = true)]
        public string AssociateDescription { get; set; }
        [SqlField("Active")]
        public bool AssociateActive { get; set; }
    }
}
