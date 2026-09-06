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
        Task VerifyOtpAsync(OtpVerify otpVerify);
        Task VerifyEmailAsync(EmailVerify emailVerify);
        Task VerifyOtpTokenAsync(OtpTokenVerify otpTokenVerify);
    }
}
