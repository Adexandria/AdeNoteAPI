using System.ComponentModel.DataAnnotations.Schema;
using TasksLibrary.Models;

namespace AdeNote.Models
{
    public class Book : BaseClass
    {
        public Book(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public Book SetUser(Guid userId)
        {
            UserId = userId;
            return this;
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public IList<Page> Pages { get; set;}
        [ForeignKey("User_id")]
        public Guid UserId{ get; set; }
        public User User { get; set; }
    }
}
