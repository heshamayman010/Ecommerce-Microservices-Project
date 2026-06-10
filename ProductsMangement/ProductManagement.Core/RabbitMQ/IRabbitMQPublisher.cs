namespace ProductManagement.Core.RabbitMQ;

public interface IRabbitMQPublisher
{
  void Publish<T>(Dictionary<string, object> headers, T message);
}
