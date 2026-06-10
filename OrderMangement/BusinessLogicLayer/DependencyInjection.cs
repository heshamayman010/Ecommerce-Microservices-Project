using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using OrderMangement.BusinessLogicLayer.Validators;
using OrderMangement.BusinessLogicLayer.Mappers;
using OrderMangement.BusinessLogicLayer.ServiceContracts;
using OrderMangement.BusinessLogicLayer.Services;
using StackExchange.Redis;
using OrderMangement.BusinessLogicLayer.RabbitMQ;



namespace OrderMangement.BusinessLogicLayer;

public static class DependencyInjection
{
  public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
  {
    // this one is used to inject all the validator in one command 
    
    services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();

    services.AddAutoMapper(typeof(OrderAddRequestToOrderMappingProfile).Assembly);

    services.AddScoped<IOrdersService, OrdersService>();

    services.AddSingleton<IRabbitMQProductNameUpdateConsumer, RabbitMQProductNameUpdateConsumer>();

    services.AddSingleton<IRabbitMQProductDeletionConsumer, RabbitMQProductDeletionConsumer>();

    services.AddHostedService<RabbitMQProductNameUpdateHostedService>();

    services.AddHostedService<RabbitMQProductDeletionHostedService>();

    services.AddStackExchangeRedisCache(options =>
    {
      options.Configuration = $"{configuration["REDIS_HOST"]}:{configuration["REDIS_PORT"]}";
    });

    return services;
  }
}
