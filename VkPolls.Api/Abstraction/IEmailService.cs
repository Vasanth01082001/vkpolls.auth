using System.Threading.Tasks;

namespace vkpolls.auth.ApiClient.Abstraction
{
    public interface IEmailService
    {
        Task<string> SendEmailVerificationAsync(string email, string token);
    }
}
