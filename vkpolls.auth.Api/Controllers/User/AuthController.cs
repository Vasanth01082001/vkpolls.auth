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
            var result = await _authConfirmation.VerifyOtpAsync(otpVerify);

            if(!result)
            {
                return BadRequest("Invalid OTP");
            }
            return Ok("OTP verified successfully");
        }

        // [HttpPost("/VerifyEmail")]
        // public async Task<IActionResult> VerifyEmail(EmailVerify emailVerify)
        // {
        //     var result = await _authConfirmation.VerifyEmailAsync(emailVerify);
        //     if (result)
        //     {
        //         return Ok("Email verified successfully");
        //     }
        //     return BadRequest("Invalid or expired token");
        // }

        [HttpGet("VerifyEmail")]
        public async Task<IActionResult> VerifyEmailViaLink([FromQuery] string email, [FromQuery] string token)
        {
            var emailVerify = new EmailVerify { email = email, token = token };
            var result = await _authConfirmation.VerifyEmailAsync(emailVerify);
            if (result)
            {
                return Ok("Email verified successfully");
            }
            return BadRequest("Invalid or expired token");
        }
    }
}
