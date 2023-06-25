using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class MultiRunOperationRequest<T>
        where T : new()
    {
        public T Arguments { get; set; } = new T();
        public IEnumerable<Guid> RunIds { get; set; } = new List<Guid> ();
    }
}
