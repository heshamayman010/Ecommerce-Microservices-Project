using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UserManagement.Infrastructure
{
public static class DependencyInjection
{
  public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
  {

    string mymainconnectionstring=configuration.GetConnectionString("DefaultConnection");
   string NewConnectionUsingEnviroment= mymainconnectionstring
   .Replace("$MYSQL_HOST",Environment.GetEnvironmentVariable("MYSQL_HOST")).
   Replace("$MYSQL_PASSWORD",Environment.GetEnvironmentVariable("MYSQL_PASSWORD")).
   Replace("$PortNumber",Environment.GetEnvironmentVariable("PortNumber"));

    services.AddDbContext<DataContext>(options => {
      options.UseMySQL(NewConnectionUsingEnviroment);
    });

    services.AddScoped<IProductsRepository, ProductsRepository>();
    return services;
  }
}
}



    // string connectionStringTemplate = configuration.GetConnectionString("DefaultConnection")!;
    // string connectionString = connectionStringTemplate
    //   .Replace("$MYSQL_HOST", Environment.GetEnvironmentVariable("MYSQL_HOST"))
    //   .Replace("$MYSQL_PASSWORD", Environment.GetEnvironmentVariable("MYSQL_PASSWORD"));

