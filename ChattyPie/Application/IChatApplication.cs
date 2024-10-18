using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Application
{
    public interface IChatApplication
    {
        Task<bool> CreateThread(Thread newThread);

        Task<bool> CreateSubThread(SubThread newSubThread);

        Task<ThreadDto> FetchThread(string threadId);
    }
}
