using System;
using System.Collections.Generic;

namespace ZzSoft.CtaLinea.Dal.Model
{
    public class UserEntity
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }

        public Guid? AssociateId { get; set; }

        public DateTime? Expiration { get; set; }
        public bool MustChangePassword { get; set; } = false;
        public bool Interactive { get; set; } = true;

        public IEnumerable<string> Roles { get; set; }
    }
}
