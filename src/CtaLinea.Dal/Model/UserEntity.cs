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

        public DateTime? ExpirationDate { get; set; }
        public bool MustChangePAssword { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}
