using OrderMangement.DataAccessLayer.RepositoryContracts;
using OrderMangement.DataAccessLayer.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace OrderMangement.DataAccessLayer;

public static class DependencyInjection
{
  public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionStringTemplate = configuration.GetConnectionString("MongoDB")!;
    string connectionString = connectionStringTemplate
      .Replace("$MONGO_HOST", Environment.GetEnvironmentVariable("MONGODB_HOST"))
      .Replace("$MONGO_PORT", Environment.GetEnvironmentVariable("MONGODB_PORT"));


// here we add the service of the mongo client as it handle it internally so we add it as singleton 
    services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

// now the mongo data base here that will be used in the repos and it returns the data base we u
    services.AddScoped<IMongoDatabase>(provider =>
    {
      IMongoClient client = provider.GetRequiredService<IMongoClient>();
      return client.GetDatabase("OrdersDatabase");
    });

    services.AddScoped<IOrdersRepository, OrdersRepository>();


    return services;
  }
}
