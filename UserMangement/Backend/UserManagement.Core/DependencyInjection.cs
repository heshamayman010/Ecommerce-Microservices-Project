using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Core.IServices;
using UserManagement.Core.Services;
using UserManagement.Core.Validators;

namespace UserManagement.Core
{
    public static class DependencyInjection
    {
            // static method must be static also


            public static IServiceCollection AddCoreServices (this IServiceCollection services)
        {
            


        services.AddTransient<IUsersService, UsersService>();
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            return services;
        }


    }


}