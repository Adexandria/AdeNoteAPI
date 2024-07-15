using AdeMessaging.Models;
using RabbitMQ.Client;

namespace AdeMessaging.Services
{
    public class RabbitMessagingService : MessagingService
    {
        public RabbitMessagingService(IMessaging messaging)
        {
            _factory = new ConnectionFactory 
            {
                HostName = messaging.HostName ?? "localhost",
                VirtualHost = messaging.VirtualHost ?? ConnectionFactory.DefaultVHost,
                UserName = messaging.UserName ?? ConnectionFactory.DefaultUser,
                Password = messaging.Password ?? ConnectionFactory.DefaultPass,
                Port = messaging.Port == 0 ? AmqpTcpEndpoint.DefaultAmqpSslPort : messaging.Port
            }; 
        }
        public override bool IsConnected()
        {
            using var connection = _factory.CreateConnection();
            if (!connection.IsOpen)
            {
                _factory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                return _factory.CreateConnection().IsOpen;
            }
            return false;
        }

        public override void Publish(string message, string topic)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: )
        }

        public override void Subscribe(string topic)
        {
            throw new NotImplementedException();
        }

        private readonly string _exchange;
        private ConnectionFactory _factory; 
    }
}
