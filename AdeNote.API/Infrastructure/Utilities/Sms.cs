namespace AdeNote.Infrastructure.Utilities
{
    public class Sms
    {
        public Sms(string phoneNumber, string message)
        {
            PhoneNumber =$"+234{phoneNumber}";
            Message = message;
        }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
