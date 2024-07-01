namespace AdeNote.Infrastructure.Utilities.SmsConfig
{
    public class SmsConfiguration
    {
        public SmsConfiguration()
        {

        }
        public SmsConfiguration(string accountKey, string accountSecret, string phonenumber)
        {
            AccountKey = accountKey;
            AccountSecret = accountSecret;
            PhoneNumber = phonenumber;
        }
        public string AccountKey { get; set; }
        public string AccountSecret { get; set; }
        public string PhoneNumber { get; set; }
    }
}
