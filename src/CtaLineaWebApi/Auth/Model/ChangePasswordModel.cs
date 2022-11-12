using System.ComponentModel.DataAnnotations;

namespace CtaLineaWebApi.Auth.Model
{
    public class ChangePasswordModel
    {
        [Required]
        public string OldPAssword { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;
    }
}
