using System.ComponentModel.DataAnnotations.Schema;
using TasksLibrary.Models;

namespace AdeNote.Models
{
    /// <summary>
    /// A book object
    /// </summary>
    public class Book : BaseClass
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        public Book()
        {

        }
        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="title">A title of the book</param>
        /// <param name="description">A description of book</param>
        public Book(string title, string description)
        {
            Title = title;
            Description = description;
        }

        /// <summary>
        /// Sets the user to the book
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Book SetUser(Guid userId)
        {
            UserId = userId;
            return this;
        }

        /// <summary>
        /// Title of the book
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of the book
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A list of pages
        /// </summary>
        public IList<Page> Pages { get; set;}

        /// <summary>
        /// An id of the user
        /// </summary>
        [ForeignKey("User_id")]
        public Guid UserId{ get; set; }

        /// <summary>
        /// A user model
        /// </summary>
        public User User { get; set; }
    }
}
