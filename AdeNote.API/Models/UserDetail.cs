using System.ComponentModel.DataAnnotations.Schema;
using TasksLibrary.Models;

namespace AdeNote.Models
{
    public class UserDetail : BaseClass
    {
        public UserDetail()
        {

        }
        public UserDetail(Guid userId,string phonenumber)
        {
            Phonenumber = phonenumber;
            IsPhoneNumberVerified = false;
            IsEmailVerified = false;
            UserId = userId;
        }

        public UserDetail SetPhonenumberVerification(bool isVerified)
        {
            IsPhoneNumberVerified = isVerified;
            return this;
        }

        public UserDetail SetEmailVerification(bool isVerified)
        {
            IsEmailVerified= isVerified;
            return this;
        }

        public string Phonenumber { get; set; }

        public bool IsPhoneNumberVerified { get; set; }

        public bool IsEmailVerified { get; set; }

        public User User { get; set; }

        [ForeignKey("User_id")]
        public Guid UserId { get; set; }
    }
}
