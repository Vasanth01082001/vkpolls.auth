using System;
using System.Collections.Generic;
using System.Text;
using vkpolls.auth.ApiClient.Abstraction;
using vkpolls.auth.ApiClient.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace vkpolls.auth.ApiClient
{
    public class SmsService : ISmsService
    {
        private readonly IApiClient _apiClient;

        public SmsService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<string> SendSmsAsync(string phoneNumber, string otpCode)
        {
            var response = await _apiClient.PostAsync<OTPStatus>(ApiEndpoints.SendSms, new { phoneNumber, otpCode });
            return response!.message;
        }
    }

    public static class ServiceExtensions
    {
        public static void AddSmsService(this IServiceCollection services, IConfiguration configuration)
        {
            var apiOptions = configuration.GetSection("ApiClients").Get<ApiClientOptions>();
            services.AddApiClientServices(options =>
            {
                options.BaseUrl = apiOptions?.BaseUrl;
                options.ApiKey = apiOptions?.ApiKey;
                options.RequestTimeoutSeconds = apiOptions?.RequestTimeoutSeconds ?? 30;
            });
            services.AddScoped<ISmsService, SmsService>();
        }
    }

    public class OTPStatus
    {
        public string message { get; set; } = string.Empty;
    }
}
