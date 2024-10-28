namespace AdeNote.Models.DTOs
{
    public class ConfirmationToken
    {
        public ConfirmationToken()
        {
            
        }
        public ConfirmationToken(string emailToken)
        {
            EmailConfirmationToken = emailToken;
        }
        public string EmailConfirmationToken { get; set; }
    }
}
