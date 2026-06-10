namespace OrderMangement.BusinessLogicLayer.RabbitMQ;

public interface IRabbitMQProductDeletionConsumer
{
  void Consume();
  void Dispose();
}

