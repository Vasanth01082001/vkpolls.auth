using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vkpolls.auth.Application.Contracts.Identity
{
    public interface IGenerateUserNameService
    {
        Task<string> GenerateUsernameFromMobile(string phoneNumber);
        Task<string> GenerateUsernameFromEmail(string email);
        Task<string> EnsureUniqueUsername(string baseUsername);
    }
}
