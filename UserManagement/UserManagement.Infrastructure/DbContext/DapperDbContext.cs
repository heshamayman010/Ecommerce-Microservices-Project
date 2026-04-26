using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace eCommerce.Infrastructure.DbContext;

public class DapperDbContext
{
  private readonly IConfiguration _configuration;
  private readonly IDbConnection _connection;

  public DapperDbContext(IConfiguration configuration)
  {
    _configuration = configuration;

    string mymainconnectionstring=configuration.GetConnectionString("PostgresConnection");
   string NewConnectionUsingEnviroment= mymainconnectionstring
   .Replace("$MyPostGrHost",Environment.GetEnvironmentVariable("MyPostGrHost")).
   Replace("$MyPostGrPassword",Environment.GetEnvironmentVariable("MyPostGrPassword")).
   Replace("$MyPostGrPort",Environment.GetEnvironmentVariable("MyPostGrPort"));

    //Create a new NpgsqlConnection with the retrieved connection string

    _connection = new NpgsqlConnection(NewConnectionUsingEnviroment);
  }

    // "PostgresConnection": "Host=$MyPostGrHost; Port=$MyPostGrPort; Database=UserMangement; Username=postgres; Password=$MyPostGrPassword"


  public IDbConnection DbConnection => _connection;
}
