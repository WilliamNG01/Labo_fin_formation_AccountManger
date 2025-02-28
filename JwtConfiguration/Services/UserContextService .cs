using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtConfiguration.Services
{
    public class UserContextService : IUserContextService
    {
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public List<string> Roles { get; private set; } = new List<string>();
        public string Token { get; private set; }

        public void SetUser(string userId, string userName, List<string> roles, string token)
        {
            UserId = userId;
            UserName = userName;
            Roles = roles;
            Token = token;
        }
    }
}
