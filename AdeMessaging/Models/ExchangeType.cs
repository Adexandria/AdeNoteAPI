using System.ComponentModel;


namespace AdeMessaging.Models
{
    public enum ExchangeType
    {
        [Description("direct")]
        direct,
        [Description("topic")]
        topicStar,
        [Description("topic")]
        topicHashTag,
        [Description("fanout")]
        fanout
    }
}
