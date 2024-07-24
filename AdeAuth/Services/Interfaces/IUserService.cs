using AdeAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Interfaces
{
    public interface IUserService<T> where T : IApplicationUser
    {
        public void SignUpUser(T user);
        public void LoginUser(T user);
        public void AddRole(T User, string RoleName);
        public void RemoveRole(T User, string RoleName);
        public IList<string> ClaimTypes(string[] claimTypes);

    }
}
