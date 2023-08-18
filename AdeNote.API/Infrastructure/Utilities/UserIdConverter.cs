using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Utilities
{
    public class UserIdConverter : ValueConverter<UserId,Guid>
    {
        public UserIdConverter() : base(v=>v.Id,v=> new UserId(v))
        {

        }
    }
}
