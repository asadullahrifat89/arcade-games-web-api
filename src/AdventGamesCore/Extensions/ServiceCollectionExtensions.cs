using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AdventGamesCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters().AddValidatorsFromAssemblyContaining<SignupCommandValidator>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var allRepositories = Assembly.GetAssembly(typeof(UserRepository))?.GetTypes().Where(type => !type.IsInterface && type.Name.EndsWith("Repository"));

            if (allRepositories is not null)
            {
                foreach (var item in allRepositories)
                {
                    Type serviceType = item.GetTypeInfo().ImplementedInterfaces.First();
                    services.AddSingleton(serviceType, item);
                }
            }

            return services;
        }

        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<IMongoDbService, MongoDbService>();
            return services;
        }
    }
}
