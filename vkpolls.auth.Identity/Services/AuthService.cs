using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using vkpolls.auth.ApiClient.Abstraction;
using vkpolls.auth.Application.Contracts.Identity;
using vkpolls.auth.Application.Exceptions;
using vkpolls.auth.Application.Models;
using vkpolls.auth.Identity.DbContext;

namespace vkpolls.auth.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGenerateUserNameService _generateUserNameService;
        private readonly VKPollsIdentityDbContext _context;
        private readonly ISmsService _smsService;
        private readonly IEmailService _emailService;

        public AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IGenerateUserNameService generateUserNameService, VKPollsIdentityDbContext context, ISmsService smsService, IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _generateUserNameService = generateUserNameService;
            _context = context;
            _smsService = smsService;
            _emailService = emailService;
        }
        public async Task LoginAsync(UserAuthIdentity userAuthIdentity)
        {
            var username = userAuthIdentity.identifier.Trim();

            var result = long.TryParse(username, out _);
            if (!result)
            {
                var user = await _userManager.FindByEmailAsync(username);

                if (user != null)
                {
                    if(!user.EmailConfirmed)
                    {
                        throw new NotFoundException("User not found.");
                    }

                    var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, userAuthIdentity.password, true, false);

                    if (!signInResult.Succeeded)
                    {
                        throw new UnauthorizedAccessException("Invalid credentials.");
                    }
                }
                else
                {
                    throw new NotFoundException("User not found.");
                }
            }
            else if (username.Length == 10)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == username);

                if (user != null)
                {
                    if(!user.PhoneNumberConfirmed)
                    {
                        throw new NotFoundException("User not found.");
                    }

                    var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, userAuthIdentity.password, true, false);

                    if (!signInResult.Succeeded)
                    {
                        throw new UnauthorizedAccessException("Invalid credentials.");
                    }
                }
                else
                {
                    throw new NotFoundException("User not found.");
                }
            }
            else
            {
                throw new BadRequestException("Invalid credentials.");
            }
        }
        public async Task RegisterAsync(UserAuthIdentity userAuthIdentity)
        {
            var username = userAuthIdentity.identifier.Trim();

            var result = long.TryParse(username, out _);
            if (!result)
            {
                var emailUser = await _userManager.FindByEmailAsync(username);
                if (emailUser != null)
                {
                    if(emailUser.EmailConfirmed)
                    {
                        throw new BadRequestException("User already exists.");
                    }
                    else
                    {

                        var ptoken = await _userManager.GeneratePasswordResetTokenAsync(emailUser);
                        await _userManager.ResetPasswordAsync(emailUser, ptoken, userAuthIdentity.password);

                        var etoken = await _userManager.GenerateEmailConfirmationTokenAsync(emailUser);
                        await _emailService.SendEmailVerificationAsync(emailUser.Email!, etoken);
                        return;
                    }
                }

                var user = new IdentityUser
                {
                    UserName = await _generateUserNameService.GenerateUsernameFromEmail(username),
                    Email = username
                };

                await CreateUserAsync(user, userAuthIdentity.password);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _emailService.SendEmailVerificationAsync(user.Email, token);
            }
            else if (username.Length == 10)
            {
                var phoneUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == username);
                if (phoneUser != null)
                {
                    if(phoneUser.PhoneNumberConfirmed)
                    {
                        throw new BadRequestException("User already exists.");
                    }
                    else
                    {
                        var passwordtoken = await _userManager.GeneratePasswordResetTokenAsync(phoneUser);
                        await _userManager.ResetPasswordAsync(phoneUser, passwordtoken, userAuthIdentity.password);

                        var ptoken = await _userManager.GenerateChangePhoneNumberTokenAsync(phoneUser, username);
                        await _smsService.SendSmsAsync(username, ptoken);
                        return;
                    }
                }

                var user = new IdentityUser
                {
                    UserName = await _generateUserNameService.GenerateUsernameFromMobile(username),
                    PhoneNumber = username
                };

                await CreateUserAsync(user, userAuthIdentity.password);
                var token = await _userManager.GenerateChangePhoneNumberTokenAsync(user, username);
                await _smsService.SendSmsAsync(username, token);
            }
            else
            {
                throw new BadRequestException("Invalid credentials.");
            }
        }

        private async Task CreateUserAsync(IdentityUser user, string password)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException(
                        "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description))
                    );
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "User");

                if (!roleResult.Succeeded)
                {
                    await transaction.RollbackAsync();
                    throw new InvalidOperationException(
                        "Role assignment failed: " + string.Join(", ", roleResult.Errors.Select(e => e.Description))
                    );
                }

                await transaction.CommitAsync();
            }
            catch
            {
                //await transaction.RollbackAsync();
                throw;
            }

        }
    }
}
