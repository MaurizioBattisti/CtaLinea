using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("n")]
    public class InternalNoteQueryItem
    {
        public string Note { get; set; } = string.Empty;
    }
}
