using eCommerce.Infrastructure.DbContext;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Core.IRepository;

namespace UserManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services)
        {

                services.AddTransient<DapperDbContext>();

    services.AddSingleton<IUsersRepository, UsersRepository>();
            return services;
        }
    }
}
