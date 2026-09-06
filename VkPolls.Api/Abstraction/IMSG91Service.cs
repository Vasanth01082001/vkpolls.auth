namespace vkpolls.auth.ApiClient.Abstraction
{
    public interface IMSG91Service
    {
        Task<string> VerifyAccessTokenAsync(string token);
    }
}
