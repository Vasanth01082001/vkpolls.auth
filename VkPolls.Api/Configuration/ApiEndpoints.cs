namespace vkpolls.auth.ApiClient.Configuration
{
    /// <summary>
    /// Centralized API endpoint constants
    /// </summary>
    public static class ApiEndpoints
    {
        // SMS Service endpoints
        public const string SendSms = "api/Otp/SendOTP";

        // Email Service endpoints
        public const string SendEmailVerification = "api/Email/SendVerification";

        // MSG91 OTP Token Verify endpoint
        public const string VerifyOtpToken = "api/v5/widget/verifyAccessToken";
    }
}
