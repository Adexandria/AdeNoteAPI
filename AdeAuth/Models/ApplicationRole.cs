using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    internal class ApplicationRole :  BaseEntity, IApplicationRole
    {
        public ApplicationRole():base()
        {
                
        }
        public ApplicationRole(string name):base() 
        { 
            Name = name;
        }


        public string Name { get; set; }

        public IList<IApplicationUser> User { get; set; } = new List<ApplicationUser>();
    }
}
