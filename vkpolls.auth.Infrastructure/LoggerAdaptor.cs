using Microsoft.Extensions.Logging;
using vkpolls.auth.Application.Contracts.Logger;

namespace vkpolls.auth.Infrastructure
{
    public class LoggerAdaptor<T> : IAppLogger<T> where T: class
    {
        private readonly ILogger<T> _logger;

        public LoggerAdaptor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<T>();
        }

        public void LogError(string message, params object[] args)
        {
            _logger.LogError(message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}
