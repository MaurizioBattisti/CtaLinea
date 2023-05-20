using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.Base
{
    public class NewUserModel
    {
        public string UserName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string? Email { get; set; }
        public DateTime? Expiration { get; set; }
        public bool MustChangePassword { get; set; }

        public Guid? AssociateId { get; set; }

        public bool Interactive { get; set; }

        public IList <string> Roles { get; set; } = new List<string>();
    }
}
