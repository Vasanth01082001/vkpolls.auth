using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Text;
using vkpolls.auth.ApiClient.Abstraction;
using vkpolls.auth.ApiClient.Configuration;

namespace vkpolls.auth.ApiClient
{
    public static class ApiClientServiceRegistration
    {
        public static IServiceCollection AddApiClientServices(this IServiceCollection services, Action<ApiClientOptions>? configureOptions = null)
        {
            // Configure API client options
            services.Configure<ApiClientOptions>(options =>
            {
                if (configureOptions != null)
                {
                    configureOptions(options);
                }
            });

            services.AddHttpClient<IApiClient, ApiClient>();

            return services;
        }
    }
}
