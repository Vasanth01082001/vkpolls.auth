using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using vkpolls.auth.ApiClient.Abstraction;
using vkpolls.auth.ApiClient.Configuration;

namespace vkpolls.auth.ApiClient
{
    public class MSG91Service : IMSG91Service
    {
        private readonly IApiClient _apiClient;

        public MSG91Service(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<string> VerifyAccessTokenAsync(string token)
        {
            var response = await _apiClient.PostAsync<OtpStatus>(ApiEndpoints.VerifyOtpToken, new { token });
            return response!.message;
        }
    }

    public static class MSG91ServiceExtensions
    {
        public static void AddOtpService(this IServiceCollection services, IConfiguration configuration)
        {
            var apiOptions = configuration.GetSection("MSG91ApiClient").Get<ApiClientOptions>();
            services.AddApiClientServices(options =>
            {
                options.BaseUrl = apiOptions?.BaseUrl;
                options.ApiKey = apiOptions?.ApiKey;
                options.RequestTimeoutSeconds = apiOptions?.RequestTimeoutSeconds ?? 30;
            });
            services.AddScoped<IMSG91Service, MSG91Service>();
        }
    }

    public class OtpStatus
    {
        public string type { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }
}
