using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Core.IServices;
using ProductManagement.Core.Mappers;
using ProductManagement.Core.RabbitMQ;
using ProductManagement.Core.Services;
using ProductManagement.Core.Validators;
using UserManagement.Infrastructure;

namespace ProductManagement.Core
{
  public static class DependencyInjection
  {

    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {

      services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

      services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();

      services.AddScoped<IProductsService, ProductsService>();
      services.AddScoped<IProductsRepository, ProductsRepository>();

    services.AddTransient<IRabbitMQPublisher, RabbitMQPublisher>();

      return services;

    }
  }

}
