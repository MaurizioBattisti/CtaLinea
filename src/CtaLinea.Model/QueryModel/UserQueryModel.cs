using CtaLinea.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("u")]
    public class UserQueryModel
    {
        [SqlField(FullText =true, SortPosition = 0)]
        public string UserName { get; set; } = string.Empty;

        [SqlField(FullText = true)]
        public string? Description { get; set; }
        [SqlField(FullText = true)]
        public string?  Email { get; set; }
        public DateTime? Expiration { get; set; }
        public bool MustChangePassword { get; set; } = true;

        public Guid? AssociateId { get; set; }
        [SqlField(FullText = true)]
        public string? AssociateDescription { get; set; }

        public bool Interactive { get; set; } = true;
        [SqlField(FullText = true)]
        public string? Roles { get; set; }

        public void CopyFrom(UserQueryModel source)
        {
            UserName = source.UserName;
            Description = source.Description;
            Email = source.Email;
            Expiration = source.Expiration;
            MustChangePassword = source.MustChangePassword;

            AssociateId = source.AssociateId;
            AssociateDescription = source.AssociateDescription;

            Interactive = source.Interactive;
            Roles = source.Roles;
        }
    }
}
