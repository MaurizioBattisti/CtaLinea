using System.ComponentModel.DataAnnotations;

namespace CtaLineaApp.Application.Model.Account
{
    public class LoginModel
    {
        [Required]
        public string? Username { get; set; } 

        [Required]
        public string? Password { get; set; } 
    }
}
