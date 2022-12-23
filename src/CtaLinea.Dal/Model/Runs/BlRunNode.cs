using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Runs
{
    internal class BlRunNode
        : RunNode
    {
        public Guid RunVariationId { get; set; }
        public string? CollectionPointDescription { get; set; }
    }
}
