using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace DotnetAPIApp.Services
{
    public class ServiceRabbitMQ : IHostedService
    {
        private IModel channel = null!;
        private IConnection connection = null!;

        private void Run()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://rabbitmq:1jj395qu@206.189.84.49:5672")
            };

            this.connection = factory.CreateConnection();
            this.channel = this.connection.CreateModel();

            // ตัวอย่างโค้ดการรับส่ง RabbitMQ https://github.com/choudhurynirjhar/rabbitmq-demo
            
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                // var contentType = e.BasicProperties.ContentType.ToString();
                var message = Encoding.UTF8.GetString(body);
                // Console.WriteLine($"Body: {message} ContentType: {contentType}");
                Console.WriteLine($"Body: {message}");

            };

            // channel.BasicConsume("akenarin-q.auth.direct", true, consumer);
            channel.BasicConsume("akenarin-q.auth.fanout", true, consumer);

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.channel.Dispose();
            this.connection.Dispose();
            return Task.CompletedTask;
        }

    }
}