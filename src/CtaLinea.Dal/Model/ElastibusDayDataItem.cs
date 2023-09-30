using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model
{
	public class ElastibusDayDataItem
	{
		public int RunCtaId { get; set; }
		public DateTime Date { get; set; }
		public float Km { get; set; }
		public int PeopleCount { get; set; }
	}
}
