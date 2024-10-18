using ChattyPie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattyPie.Infrastructure.Interfaces
{
    public interface IRepository<T> where T : BaseClass
    {
        Task<bool> Add(T thread);
        Task<bool> Update(T thread);
    }
}
