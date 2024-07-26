using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    internal class Role : ApplicationRole
    {
        public Role()
        {
           Id = Guid.NewGuid();
        }


        public Role(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        public new Guid Id { get; set; }
        public new string Name { get; set; }
    }
}
