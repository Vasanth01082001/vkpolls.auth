namespace vkpolls.auth.ApiClient.Configuration
{
    /// <summary>
    /// Options for configuring the API client
    /// </summary>
    public class ApiClientOptions
    {
        /// <summary>
        /// The base URL for the API client
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// API key for authentication (if required)
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Request timeout in seconds
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = 30;
    }
}
