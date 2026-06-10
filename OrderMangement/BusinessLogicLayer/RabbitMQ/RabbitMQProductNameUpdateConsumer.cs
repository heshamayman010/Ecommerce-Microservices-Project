using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderMangement.BusinessLogicLayer.RabbitMQ;

public class RabbitMQProductNameUpdateConsumer : IDisposable, IRabbitMQProductNameUpdateConsumer
{
  private readonly IConfiguration _configuration;
  private readonly IModel _channel;
  private readonly IConnection _connection;
  private readonly ILogger<RabbitMQProductNameUpdateConsumer> _logger;

  public RabbitMQProductNameUpdateConsumer(IConfiguration configuration, ILogger<RabbitMQProductNameUpdateConsumer> logger)
  {
  _configuration = configuration;

  string hostName = _configuration["RabbitMQ_HostName"]!;
  string userName = _configuration["RabbitMQ_UserName"]!;
  string password = _configuration["RabbitMQ_Password"]!;
  string port = _configuration["RabbitMQ_Port"]!;
  _logger = logger;

  ConnectionFactory connectionFactory = new ConnectionFactory()
  {
    HostName = hostName,
    UserName = userName,
    Password = password,
    Port = Convert.ToInt32(port)
  };

  int retries = 0;
  while (retries < 30)
  {
    try
    {
      _connection = connectionFactory.CreateConnection();
      break; 
    }
    catch (Exception ex)
    {
      retries++;
      _logger.LogWarning($"RabbitMQ not ready for ProductDeletionConsumer (attempt {retries}/30). Waiting 5 seconds... Error: {ex.Message}");
      Thread.Sleep(5000);
      if (retries >= 30)
      {
        _logger.LogError("Failed to connect to RabbitMQ after 30 attempts");
        throw;
      }
    }
  }

  _channel = _connection.CreateModel();
  }


  public void Consume()
  {
    //string routingKey = "product.update.name";
    string queueName = "orders.product.update.name.queue";

    //Create exchange
    string exchangeName = _configuration["RabbitMQ_Products_Exchange"]!;
    _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);
    string routingKey = "product.update.name";

    //Create message queue
    _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null); //x-message-ttl | x-max-length | x-expired 

    //Bind the message to exchange
    _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);


    EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

    consumer.Received += (sender, args) => 
    {
      byte[] body = args.Body.ToArray();
      string message = Encoding.UTF8.GetString(body);

      if (message != null)
      {
        ProductNameUpdateMessage? productNameUpdateMessage = JsonSerializer.Deserialize<ProductNameUpdateMessage>(message);

        if (productNameUpdateMessage != null)
        {
          _logger.LogInformation($"Product name updated: {productNameUpdateMessage.ProductID}, New name: {productNameUpdateMessage.NewName}");
        }
      }
    };

    _channel.BasicConsume(queue: queueName, consumer: consumer, autoAck: true);
  }

  public void Dispose()
  {
    _channel.Dispose();
    _connection.Dispose();
  }
}
