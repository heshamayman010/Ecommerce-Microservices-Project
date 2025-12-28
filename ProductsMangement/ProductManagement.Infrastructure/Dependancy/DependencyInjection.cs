using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Infrastructure;

namespace UserManagement.Infrastructure
{
public static class DependencyInjection
{
  public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
  {

    services.AddDbContext<DataContext>(options => {
      options.UseMySQL(configuration.GetConnectionString("DefaultConnection")!);
    });

    services.AddScoped<IProductsRepository, ProductsRepository>();
    return services;
  }
}
}
