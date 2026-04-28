using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using OrderMangement.BusinessLogicLayer.Validators;
using OrderMangement.BusinessLogicLayer.Mappers;
using OrderMangement.BusinessLogicLayer.ServiceContracts;
using OrderMangement.BusinessLogicLayer.Services;


namespace OrderMangement.BusinessLogicLayer;

public static class DependencyInjection
{
  public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
  {
    // this one is used to inject all the validator in one command 
    
    services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();

    services.AddAutoMapper(typeof(OrderAddRequestToOrderMappingProfile).Assembly);

    services.AddScoped<IOrdersService, OrdersService>();
    return services;
  }
}
