﻿using AdeAuth.Models;
using RestSharp.Authenticators.OAuth;
using System.ComponentModel.DataAnnotations.Schema;


namespace AdeNote.Models
{
    public class User : BaseClass, IApplicationUser
    {
        protected User() { }

        public User(string firstName, string lastName, string email, AuthType authType)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AuthenticationType = authType;
        }


        public void EnableTwoFactor(MFAType twoFactorType, string authenticatorKey = null)
        {
            TwoFactorEnabled = true;
            TwoFactorType = (int)twoFactorType;
            AuthenticatorKey = authenticatorKey;
        }

        public void DisableTwoFactor()
        {
            TwoFactorEnabled = false;
            TwoFactorType = 0;
            AuthenticatorKey = null;
        }

        public void SetPassword(string hashPassword)
        {
            PasswordHash = hashPassword;
        }

        public void ConfirmEmailVerification()
        {
            EmailConfirmed = true;
        }

        public void ConfirmPhoneNumberVerification()
        {
            PhoneNumberConfirmed = true;
        }

        public void EnableLockOut()
        {
            LockoutEnabled = true;
        }


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PasswordHash { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public int TwoFactorType { get; set; }
        public string? AuthenticatorKey { get; set; }
        public bool LockoutEnabled { get; set; }
        public AuthType AuthenticationType { get; set; }
        public IList<Book> Books { get; set; } = new List<Book>();
    }
}