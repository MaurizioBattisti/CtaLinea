using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Model
{
    public class ImportDetailQueryItem
        : TtServiceQueryItem
    {
        public Guid ImportId { get; set; }
        public int ServiceId { get; set; }

        public string? ImportNote { get; set; }
    }
}
