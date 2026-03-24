using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using vkpolls.auth.Application.Contracts.Identity;
using vkpolls.auth.Application.Models;

namespace vkpolls.auth.Identity.Services
{
    public class AuthConfirmation : IAuthConfirmation
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthConfirmation(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> VerifyOtpAsync(OtpVerify otpVerify)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == otpVerify.phoneNumber);
            var valid = user != null ? await _userManager.VerifyChangePhoneNumberTokenAsync(user, otpVerify.otpCode, otpVerify.phoneNumber) : false;

            if (!valid) return false;

            user!.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> VerifyEmailAsync(EmailVerify emailVerify)
        {
            var user = await _userManager.FindByEmailAsync(emailVerify.email);
            if (user == null) return false;

            var valid = await _userManager.ConfirmEmailAsync(user, emailVerify.token);
            return valid.Succeeded;
        }
    }
}
