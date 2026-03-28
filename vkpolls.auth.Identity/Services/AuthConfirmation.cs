using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using vkpolls.auth.Application.Contracts.Identity;
using vkpolls.auth.Application.Exceptions;
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

        public async Task VerifyOtpAsync(OtpVerify otpVerify)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == otpVerify.phoneNumber);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (user.PhoneNumberConfirmed)
                throw new BadRequestException("Phone number is already confirmed.");

            var isValid = await _userManager.VerifyChangePhoneNumberTokenAsync(
                user, otpVerify.otpCode, otpVerify.phoneNumber);

            if (!isValid)
                throw new BadRequestException("Invalid or expired OTP.");

            user.PhoneNumberConfirmed = true;
            var result = await _userManager.UpdateAsync(user);
        }

        public async Task VerifyEmailAsync(EmailVerify emailVerify)
        {
            var user = await _userManager.FindByEmailAsync(emailVerify.email);

            if (user == null)
                throw new NotFoundException("User not found.");

            if (user.EmailConfirmed)
                throw new BadRequestException("Email is already confirmed.");

            var result = await _userManager.ConfirmEmailAsync(user, emailVerify.token);

            if(!result.Succeeded)
            {
                throw new BadRequestException("Invalid or expired Link.");
            }
        }
    }
}
