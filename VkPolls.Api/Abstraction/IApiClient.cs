using System;
using System.Collections.Generic;
using System.Text;

namespace vkpolls.auth.ApiClient.Abstraction
{
    public interface IApiClient
    {
        Task<T?> GetAsync<T>(string url, object? query = null);
        Task<T?> PostAsync<T>(string url, object? body = null);
        Task<bool> DeleteAsync(string url);
    }
}
