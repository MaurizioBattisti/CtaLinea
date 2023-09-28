using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model
{
    public class ElastibusDayExport
    {
        public Guid  RunId { get; set; }
        public DateTime Day { get; set; }
        public float? Km { get; set; }
        public int? PeopleCount { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
