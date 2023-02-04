using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Helpers
{
    public class CheckResult
    {
        public CheckStatus Status { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public IEnumerable<CheckResultItem>? Errors { get; set; }
        public IEnumerable<CheckResultItem>? Warnings { get; set; }
        public IEnumerable<CheckResultItem>? Informations { get; set; }
    }
}
