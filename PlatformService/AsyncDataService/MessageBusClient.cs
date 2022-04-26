using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataService
{
    public class MessageBusClient:IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"]) };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange:"Trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine(" --> Connected to message bus"); 
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($" --> MessageBusClient: Failed to connect to MessageBus: {ex.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($" --> MessageBusClient: RabbitMQ connection shutdown: {e.ReplyText}");
        }

        public void PublishNewPlatform(PlatformPublishedDto platform)
        {
            var message = JsonSerializer.Serialize(platform);
            if(_connection.IsOpen)
            {
                Console.WriteLine($" --> Message queue open. Publishing new platform: {message}");   
                SendMessage(message);
            }
            else
            {
                Console.WriteLine($" --> Message queue closed. Cannot publish new platform: {message}");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "Trigger",routingKey: "",basicProperties: null,body: body);
            Console.WriteLine($" --> Message sent to message bus: {message}");
        }
        public void Dispose()
        {
            Console.WriteLine(" --> Disposing MessageBusClient");
            if(_channel.IsOpen)
            {
                _channel.Close();   
                _connection.Close();
            }
        }
    }
  
}
