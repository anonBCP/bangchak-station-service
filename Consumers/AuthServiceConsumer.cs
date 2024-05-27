using BangchakStationService.Models;
using BangchakStationService.ModelsDto;
using BangchakStationService.Services.RabbitMQ;
using BangchakStationService.Services.UserService;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace BangchakStationService.Consumers
{
    public class AuthServiceConsumer : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMQConnectionManager _connectionManager;

        public AuthServiceConsumer(IServiceProvider serviceProvider, IRabbitMQConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
            _serviceProvider = serviceProvider;
        }

        private void Run()
        {
            // ตัวอย่างโค้ดการรับส่ง RabbitMQ https://github.com/choudhurynirjhar/rabbitmq-demo

            var channel = _connectionManager.GetChannel();
            string userId = null!;

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, e) =>
            {
                try
                {
                    var body = e.Body.ToArray();
                    var propertieType = e.BasicProperties.Type.ToString();
                    if (propertieType == "UserCreated")
                    {
                        var message = Encoding.UTF8.GetString(body);
                        var user = JsonConvert.DeserializeObject<UserMessage>(message);

                        userId = user!.UserId;

                        var newUser = new User
                        {
                            UserId = user!.UserId,
                            Fullname = user.Fullname
                        };

                        // insert user
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                            await userService.CreateUserAsync(newUser);
                            await userService.SaveChangesAsync();
                        }

                        Console.WriteLine($"{user!.UserId} {user.Fullname}");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error processing message: {0}", ex.Message);
                    var message = new { errorMessage = true };
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                    var properties = channel.CreateBasicProperties();
                    properties.CorrelationId = userId;
                    channel.BasicPublish(exchange: "akenarin.auth.error.ex",
                                         routingKey: "auth-error",
                                         basicProperties: null,
                                         body: body);
                }
            };

            channel.BasicConsume("akenarin.auth.q", true, consumer);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
