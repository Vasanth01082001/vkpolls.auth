using Microsoft.AspNetCore.Identity;
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
        public async Task<string> LoginAsync(UserAuthIdentity userAuthIdentity)
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
                        throw new BadRequestException("Confirm email before login.");
                    }

                    var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, userAuthIdentity.password, true, false);

                    if (!signInResult.Succeeded)
                    {
                        throw new UnauthorizedAccessException("Invalid credentials.");
                    }
                }
                else
                {
                    throw new NotFoundException("User with the provided email not found.");
                }
            }
            else if (username.Length == 10)
            {
                var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == username);

                if (user != null)
                {
                    if(!user.PhoneNumberConfirmed)
                    {
                        throw new BadRequestException("Confirm mobile number before login");
                    }

                    var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, userAuthIdentity.password, true, false);

                    if (!signInResult.Succeeded)
                    {
                        throw new UnauthorizedAccessException("Invalid credentials.");
                    }
                }
                else
                {
                    throw new NotFoundException("User with the provided phone number not found.");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }
            return "Login successful.";
        }
        public async Task<string> RegisterAsync(UserAuthIdentity userAuthIdentity)
        {
            var username = userAuthIdentity.identifier.Trim();

            var result = long.TryParse(username, out _);
            if (!result)
            {
                var emailUser = await _userManager.FindByEmailAsync(username);
                if (emailUser != null)
                {
                    throw new BadRequestException("A user with this email already exists.");
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
                var phoneUser = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == username);
                if (phoneUser != null)
                {
                    throw new BadRequestException("A user with this phone number already exists.");
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
            return "Registered successfully.";
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
