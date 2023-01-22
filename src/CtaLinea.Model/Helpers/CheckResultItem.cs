using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Helpers
{
    public class CheckResultItem
    {
        public string Category { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid? Id { get; set; }
    }
}
