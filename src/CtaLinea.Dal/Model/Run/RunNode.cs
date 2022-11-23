using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Run
{
    public class RunNode
    {
        public Guid RunNodeId { get; set; }
        public string CollectionPointId { get; set; }

        public TimeSpan Hout { get; set; }
        public int ProgrNumber { get; set; }

        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
    }
}
