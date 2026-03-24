using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using vkpolls.auth.ApiClient.Abstraction;
using vkpolls.auth.ApiClient.Configuration;
using Microsoft.Extensions.Options;

namespace vkpolls.auth.ApiClient
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private readonly ApiClientOptions _options;

        public ApiClient(HttpClient client, IOptions<ApiClientOptions> options)
        {
            _client = client;
            _options = options.Value;
        }

        public async Task<T?> GetAsync<T>(string url, object? query = null)
        {
            var fullUrl = BuildUrl(url);
            var response = await _client.GetAsync(fullUrl);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<T?> PostAsync<T>(string url, object? body = null)
        {
            var fullUrl = BuildUrl(url);
            var response = await _client.PostAsJsonAsync(fullUrl, body);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<bool> DeleteAsync(string url)
        {
            var fullUrl = BuildUrl(url);
            var response = await _client.DeleteAsync(fullUrl);

            return response.IsSuccessStatusCode;
        }

        private string BuildUrl(string endpoint)
        {
            if (string.IsNullOrEmpty(_options.BaseUrl))
            {
                throw new InvalidOperationException("BaseUrl is not configured in ApiClientOptions.");
            }

            var baseUrl = _options.BaseUrl.TrimEnd('/');
            var endpoint_trimmed = endpoint.TrimStart('/');
            return $"{baseUrl}/{endpoint_trimmed}";
        }
    }

}
