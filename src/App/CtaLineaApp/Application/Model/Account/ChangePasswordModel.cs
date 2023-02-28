using System.ComponentModel.DataAnnotations;

namespace CtaLineaApp.Application.Model.Account
{
    public class ChangePasswordModel
    {
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewdPassword { get; set; }
        [Required]
        public string? ConfirmdPassword { get; set; }
    }
}
