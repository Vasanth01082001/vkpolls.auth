using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using vkpolls.auth.Application.Contracts.Identity;
using vkpolls.auth.Application.Models;

namespace vkpolls.auth.Api.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthConfirmation _authConfirmation;

        public AuthController(IAuthService authService, IAuthConfirmation authConfirmation)
        {
            _authService = authService;
            _authConfirmation = authConfirmation;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> UserRegistration(UserAuthIdentity userAuthIdentity)
        {
            await _authService.RegisterAsync(userAuthIdentity);
            return Ok("User Registered Successfully");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin(UserAuthIdentity userAuthIdentity)
        {
            await _authService.LoginAsync(userAuthIdentity);
            return Ok("User logged in Successfully");
        }

        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(OtpVerify otpVerify)
        {
            await _authConfirmation.VerifyOtpAsync(otpVerify);
            return Ok("OTP verified successfully");
        }

        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VerifyEmailViaLink([FromQuery] string email, [FromQuery] string token)
        {
            var emailVerify = new EmailVerify { email = email, token = token };
            await _authConfirmation.VerifyEmailAsync(emailVerify);
            return Ok("Email verified successfully");
        }
    }
}
