using RabbitMQ.Client;

namespace BangchakStationService.Services.RabbitMQ
{
    public interface IRabbitMQConnectionManager
    {
            IConnection GetConnection();
            IModel GetChannel();
    }
}
