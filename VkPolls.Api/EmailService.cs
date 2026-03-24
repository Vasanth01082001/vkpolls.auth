using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using vkpolls.auth.ApiClient.Abstraction;
using vkpolls.auth.ApiClient.Configuration;

namespace vkpolls.auth.ApiClient
{
    public class EmailService : IEmailService
    {
        private readonly IApiClient _apiClient;

        public EmailService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<string> SendEmailVerificationAsync(string email, string token)
        {
            var response = await _apiClient.PostAsync<EmailStatus>(ApiEndpoints.SendEmailVerification, new { email, token });
            return response!.message;
        }
    }

    public static class EmailServiceExtensions
    {
        public static void AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            var apiOptions = configuration.GetSection("ApiClients").Get<ApiClientOptions>();
            services.AddApiClientServices(options =>
            {
                options.BaseUrl = apiOptions?.BaseUrl;
                options.ApiKey = apiOptions?.ApiKey;
                options.RequestTimeoutSeconds = apiOptions?.RequestTimeoutSeconds ?? 30;
            });
            services.AddScoped<IEmailService, EmailService>();
        }
    }

    public class EmailStatus
    {
        public string message { get; set; } = string.Empty;
    }
}
