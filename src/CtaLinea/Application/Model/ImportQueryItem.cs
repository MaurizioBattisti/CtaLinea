using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLinea.Application.Model
{
    public class ImportQueryItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }

        public string Note { get; set; }
        public string User { get; set; }

        public DateTime ImportStartDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public string ImportStatus { get; set; }

        public int ImportedElements { get; set; }
        public int ProcessedElements { get; set; }
    }
}
