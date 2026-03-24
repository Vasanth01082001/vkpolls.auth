using System;
using System.Collections.Generic;
using System.Text;

namespace vkpolls.auth.ApiClient.Abstraction
{
    public interface ISmsService
    {
        Task<string> SendSmsAsync(string phoneNumber, string otpCode);
    }
}
