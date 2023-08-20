using System.ComponentModel.DataAnnotations.Schema;
using TasksLibrary.Models;

namespace AdeNote.Models
{
    /// <summary>
    /// User detail object
    /// </summary>
    public class UserDetail : BaseClass
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public UserDetail()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="phonenumber">Phone number</param>
        public UserDetail(Guid userId,string phonenumber)
        {
            Phonenumber = phonenumber;
            IsPhoneNumberVerified = false;
            IsEmailVerified = false;
            UserId = userId;
        }

        /// <summary>
        /// Verify the phone number
        /// </summary>
        /// <param name="isVerified">A boolean value</param>
        /// <returns>User detail</returns>
        public UserDetail VerifyPhoneNumber()
        {
            IsPhoneNumberVerified = true;
            return this;
        }

        /// <summary>
        /// Verify Email of the user
        /// </summary>
        /// <param name="isVerified">A boolean value</param>
        /// <returns>User detail</returns>
        public UserDetail VerifyEmail(bool isVerified)
        {
            IsEmailVerified= isVerified;
            return this;
        }

        /// <summary>
        /// Phone number of the user
        /// </summary>
        public string Phonenumber { get; set; }

        /// <summary>
        /// Shows if the number has been verified
        /// </summary>
        public bool IsPhoneNumberVerified { get; set; }

        /// <summary>
        /// Shows if the email has been verified
        /// </summary>
        public bool IsEmailVerified { get; set; }


        /// <summary>
        /// User
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [ForeignKey("User_id")]
        public Guid UserId { get; set; }
    }
}
