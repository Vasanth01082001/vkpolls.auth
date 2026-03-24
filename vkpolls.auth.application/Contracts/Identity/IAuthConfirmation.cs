using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vkpolls.auth.Application.Models;

namespace vkpolls.auth.Application.Contracts.Identity
{
    public interface IAuthConfirmation
    {
        Task<bool> VerifyOtpAsync(OtpVerify otpVerify);
        Task<bool> VerifyEmailAsync(EmailVerify emailVerify);
    }
}
