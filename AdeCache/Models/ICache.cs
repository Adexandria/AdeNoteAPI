using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeCache.Models
{
    public interface ICache
    {
        IMemoryCache MemoryCache { get; set; }
        string HostName {  get; set; }
    }
}
