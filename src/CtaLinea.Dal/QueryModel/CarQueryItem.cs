using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("c")]
    public class CarQueryItem
    {
        [SqlField("CarId")]
        public Guid Id { get; set; }

        [SqlField(FullText =true )]
        public string Description { get; set; }
        [SqlField(FullText = true)]
        public string RegNumber { get; set; }

        public int NrSittings { get; set; }

        [SqlField(FullText = true)]
        public string BsCarId { get; set; }
        
        [SqlField(FullText = true)]
        public string ChassisNumber { get; set; }
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
        public string AssociateDescription { get; set; }
        [SqlAlias("a")]
        [SqlField("Active")]
        public bool AssociateActive { get; set; }
    }
}
