using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using vkpolls.auth.Application.Contracts.Identity;

namespace vkpolls.auth.Identity.Services
{
    public class GenerateUserNameService : IGenerateUserNameService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public GenerateUserNameService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GenerateUsernameFromMobile(string phoneNumber)
        {
            phoneNumber = new string(phoneNumber.Where(char.IsDigit).ToArray()); // sanitize

            string lastDigits = phoneNumber.Length > 6
                                ? phoneNumber[^6..]
                                : phoneNumber;

            string random = Guid.NewGuid().ToString("N")[..4].ToUpper();

            return $"mob_{lastDigits}_{random}";
        }

        public async Task<string> GenerateUsernameFromEmail(string email)
        {
            string prefix = email.Split('@')[0];

            // remove invalid characters
            foreach (var c in new[] { '.', '-', '+', '_' })
                prefix = prefix.Replace(c.ToString(), "");

            string random = Guid.NewGuid().ToString("N")[..4].ToUpper();

            return $"{prefix}_{random}";
        }

        public async Task<string> EnsureUniqueUsername(string baseUsername)
        {
            string username = baseUsername;

            while (await _userManager.FindByNameAsync(username) != null)
            {
                string random = Guid.NewGuid().ToString("N")[..4].ToUpper();
                username = $"{baseUsername}_{random}";
            }

            return username;
        }
    }
}
