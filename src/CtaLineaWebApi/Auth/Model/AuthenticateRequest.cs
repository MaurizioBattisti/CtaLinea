

using System;
using System.ComponentModel.DataAnnotations;

namespace CtaLineaWebApi.Auth.Model
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}