using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeAuth.Models
{
    public interface IRefreshToken
    {
        Guid Id { get; set; }

        string Token { get; set; }

        Guid UserId { get; set; }

        DateTime ExpiryDate { get; set; }

        bool IsRevoked { get; set; }
    }
}
