using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Response
{
    public class MultiRunOperationResponse
        : OperationResponse
    {
        public IDictionary<Guid, OperationResponse>? Detail { get; set; } = null;
    }
}
