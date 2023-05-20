using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.Base
{
    public class ResetUserPasswordModel
    {
        public string Password { get; set; } = string.Empty;
        public bool SetMustChange { get; set; } = false;
    }
}
