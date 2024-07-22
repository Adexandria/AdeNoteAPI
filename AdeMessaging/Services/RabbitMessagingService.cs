using AdeMessaging.Models;
using AdeMessaging.Services.Extensions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using ExchangeType = AdeMessaging.Models.ExchangeType;

namespace AdeMessaging.Services
{
    public class RabbitMessagingService : MessagingService
    {
        public RabbitMessagingService(IMessaging messaging, ILoggerFactory loggerFactory)
        {
            _factory = new ConnectionFactory 
            {
                HostName = messaging.HostName ?? "localhost",
                VirtualHost = messaging.VirtualHost ?? ConnectionFactory.DefaultVHost,
                UserName = messaging.UserName ?? ConnectionFactory.DefaultUser,
                Password = messaging.Password ?? ConnectionFactory.DefaultPass,
                Port = messaging.Port == 0 ? AmqpTcpEndpoint.UseDefaultPort : messaging.Port
            };
            _exchangeType = messaging.ExchangeType;
            _logger = loggerFactory.CreateLogger<RabbitMessagingService>();
        }
        public override bool IsConnected()
        {
            try
            {
                using var connection = _factory.CreateConnection();
                _logger.LogInformation("Connected to rabbit mq");
            }
            catch (BrokerUnreachableException)
            {
               _logger.LogWarning("Failed to connect rabbit mq, trying localhost connection");
               _factory = new ConnectionFactory
               {
                 HostName = "localhost"
               };

               var isOpen = _factory.CreateConnection().IsOpen;

               _logger.LogInformation("Rabbitmq localhost connection:{Status}", isOpen);

               return isOpen;
            }
          
            return true;
        }

        public override string Publish(string message, 
            string exchange, string routingKey)
        {
            using var connection = _factory.CreateConnection();

            using var channel = connection.CreateModel();

            var exchangeType = _exchangeType.GetDescription();

            var encodedMessage = Encoding.UTF8.GetBytes(message);

            _logger.LogInformation("Preparing to publish message");

            return PublishMessage(channel, exchange, routingKey, encodedMessage);
        }
      
        public override void CreateExchange(string exchange)
        {
            using var connection = _factory.CreateConnection();

            using var channel = connection.CreateModel();

            var exchangeType = _exchangeType.GetDescription();

            _logger.LogInformation("Declaring {ExchangeType} for publishing", exchangeType);

            channel.ExchangeDeclare(exchange: exchange, type: exchangeType, durable: true, autoDelete: false);
        }

        public override void CreateQueue(string queue,string exchange, string routingKey)
        {
            using var connection = _factory.CreateConnection();

            using var channel = connection.CreateModel();

            var exchangeType = _exchangeType.GetDescription();

            _logger.LogInformation("Declaring {ExchangeType} for publishing", exchangeType);

            channel.ExchangeDeclare(exchange: exchange, type: exchangeType, durable: true, autoDelete: false);

            var queueName = channel.QueueDeclare(queue:queue,autoDelete: false, durable: true,exclusive:false);

            _logger.LogInformation("Binding channel to {queue} queue", queueName);

            BindQueue(channel, exchange, queueName, routingKey);
        }

        public override string Subscribe(string exchange, string queue, string routingKey)
        {
            var result = string.Empty;

            using var connection = _factory.CreateConnection();

            using var channel = connection.CreateModel();

            BindQueue(channel, exchange, queue, routingKey);

            var consumer = new EventingBasicConsumer(channel);

            _logger.LogInformation("Waiting for message from publisher");

            var consumerResult = channel.BasicGet(queue, true);

            if(consumerResult != null)
            {
                var body = consumerResult.Body.ToArray();
                result = Encoding.UTF8.GetString(body);
            }
            
            channel.BasicConsume(queue: queue,
                    autoAck: true,
                    consumer: consumer); 

            return result;
        }

        private void BindQueue(IModel channel,string exchange, string queue, string routingKey)
        {
            switch (_exchangeType)
            {
                case ExchangeType when _exchangeType == ExchangeType.fanout:
                    channel.QueueBind(queue: queue,
                        exchange: exchange,
                        routingKey: string.Empty);
                    break;
                default:
                    channel.QueueBind(queue: queue,
                                exchange: exchange,
                                routingKey: routingKey);
                break;
            }
        }



        private string PublishMessage(IModel channel, 
            string queue, string routingKey, 
            byte[] encodedMessage)
        {
            switch(_exchangeType)
            {
               case ExchangeType when  _exchangeType == ExchangeType.fanout:
                    channel.BasicPublish(exchange: queue,
                      routingKey: string.Empty,
                      basicProperties: null,
                      body: encodedMessage);
                    break;
               case ExchangeType when _exchangeType == ExchangeType.direct:
                    channel.BasicPublish(exchange: queue,
                        routingKey:  routingKey,
                        basicProperties:null,
                        body: encodedMessage);
               break;
               case ExchangeType when _exchangeType == ExchangeType.topicHashTag:
                    var routingKey1 = routingKey.Contains(".#") ? routingKey : routingKey+=".#";
                    channel.BasicPublish(exchange: queue,
                        routingKey: routingKey1,
                        basicProperties: null,
                        body: encodedMessage);
                    break;
               default:
                    var routingKey2 = routingKey.Contains(".*") ? routingKey : routingKey = "*."+routingKey+"*.";
                    channel.BasicPublish(exchange: queue,
                        routingKey: routingKey2,
                        basicProperties: null,
                        body: encodedMessage);
               break;
            }

            _logger.LogInformation("Message has been published, Routing key:{RoutingKey}",routingKey);

            return routingKey;
        }


        private readonly ExchangeType _exchangeType;
        private ConnectionFactory _factory; 
        private readonly ILogger<RabbitMessagingService> _logger;
    }
}
