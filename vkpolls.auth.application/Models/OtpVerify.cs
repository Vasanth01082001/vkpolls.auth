using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vkpolls.auth.Application.Models
{
    public class OtpVerify
    {
        [Required]
        public string phoneNumber { get; set; } = string.Empty;
        [Required]
        public string otpCode { get; set; } = string.Empty;
    }
}
