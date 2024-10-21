namespace AdeNote.Infrastructure.Utilities
{
    public class Cdn
    {
        public Cdn(string endpoint)
        {
            Endpoint = endpoint;
        }
        public string Endpoint { get; set; }
    }
}