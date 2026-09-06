using System.ComponentModel.DataAnnotations;

namespace vkpolls.auth.Application.Models
{
    public class OtpTokenVerify
    {
        [Required]
        public string phoneNumber { get; set; } = string.Empty;
        [Required]
        public string otpToken { get; set; } = string.Empty;
    }
}
