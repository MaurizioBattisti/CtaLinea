using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class ApplyChangeToSimRequest<T>
        where T : class, new()
    {
        public ApplyChangesSimulationRequest ApplyChanges { get; set; } = new ApplyChangesSimulationRequest();
        public T Filter { get; set; } = new T();
    }
}
