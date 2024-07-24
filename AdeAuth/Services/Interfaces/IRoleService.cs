using AdeAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Services.Interfaces
{
    public interface IRoleService
    {
        void CreateRoles(string[] roles);
        void CreateRole(string roles);
        void DeleteRoles(string[] roles);
        void DeleteRole(string name);
    }
}
