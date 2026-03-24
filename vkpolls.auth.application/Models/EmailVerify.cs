using System.ComponentModel.DataAnnotations;

namespace vkpolls.auth.Application.Models
{
    public class EmailVerify
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        [Required]
        public string token { get; set; } = string.Empty;
    }
}
