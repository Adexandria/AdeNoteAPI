namespace AdeNote.Infrastructure.Utilities.EventSystem
{
    public class EventConfiguration
    {
        public string Exchange {  get; set; }
        public string RoutingKey { get; set; }
        public string Queue {  get; set; }
    }
}
