using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Runs
{
    internal class BlCarReplacementDetail
    {
        public Guid CarReplacementId { get; set; }
        public Guid OriginaRunCarId { get; set;}
        public Guid ReplacedRunCarId { get; set; }
    }
}
