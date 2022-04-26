using System.Text;
using CommandService.EventProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
        }
        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration.GetValue<string>("RabbitMQ:HostName"),
                Port = int.Parse(_configuration.GetValue<string>("RabbitMQ:Port"))
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _configuration.GetValue<string>("RabbitMQ:ExchangeName"), type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: _configuration.GetValue<string>("RabbitMQ:ExchangeName"), routingKey: "");
            Console.WriteLine("  --> Listeing to the message bus...");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("  --> RabbitMQ connection shutdown...");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel); 
            consumer.Received += (moduleHandle, ea) =>
            {
                Console.WriteLine("  --> Received message from the message bus...");
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(message);
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}