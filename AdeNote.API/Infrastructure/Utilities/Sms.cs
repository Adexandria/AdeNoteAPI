namespace AdeNote.Infrastructure.Utilities
{
    public class Sms
    {
        public Sms(string phoneNumber, string message)
        {
            PhoneNumber = phoneNumber;
            Message = message;
        }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
