using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Contab
{
	public class MultiRunForfaitOperation
	{
		public int? ForfaitId { get; set; }
		public IEnumerable<Guid>? RunIds { get; set; }
	}
}
