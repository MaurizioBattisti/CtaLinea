using System;
using System.Collections.Generic;
using ZzSoft.CtaLinea.Dal.Model;

namespace CtaLineaWebApi.Auth.Model
{
    public class AuthenticateResponse
    {
        public string Description { get; set; }
        public string Username { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public bool MustChangePAssword { get; set; } = false;
        public IEnumerable<string> Roles { get; set; } = new string[] { };
        public string Token { get; set; } = String.Empty;
        public DateTime Expiration { get; set; }
        public Guid? AssociateId { get; set; }

        public AuthenticateResponse(
            UserEntity user,
            string token,
            DateTime exipiration)
        {
            Description = user.Description;
            Username = user.UserName;
            Email = user.Email;
            MustChangePAssword = user.MustChangePassword;

            Roles = user.Roles;
            AssociateId = user.AssociateId;
            Token = token;
            Expiration = exipiration;
        }
    }
}