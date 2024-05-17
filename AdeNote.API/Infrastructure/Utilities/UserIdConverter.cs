using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdeNote.Infrastructure.Utilities
{
    /// <summary>
    /// A custom converter to convert the guid into an object
    /// </summary>
    public class UserIdConverter : ValueConverter<UserId,Guid>
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public UserIdConverter() : base(v => v.Id,v => new UserId(v))
        {

        }
    }
}
