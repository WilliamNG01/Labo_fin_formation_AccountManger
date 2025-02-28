using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtConfiguration.Services
{
    public interface IUserContextService
    {
        string UserId { get; }
        string UserName { get; }
        List<string> Roles { get; }
        string Token { get; }
        void SetUser(string userId, string userName, List<string> roles, string token);
    }
}
