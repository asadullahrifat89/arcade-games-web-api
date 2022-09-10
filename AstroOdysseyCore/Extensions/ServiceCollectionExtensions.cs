using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AstroOdysseyCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddValidators(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(SignupCommandValidator).GetTypeInfo().Assembly));

            return serviceCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            var allRepositories = Assembly.GetAssembly(typeof(UserRepository))?.GetTypes().Where(type => !type.IsInterface && type.Name.EndsWith("Repository"));

            if (allRepositories is not null)
            {
                foreach (var item in allRepositories)
                {
                    Type serviceType = item.GetTypeInfo().ImplementedInterfaces.First();
                    serviceCollection.AddSingleton(serviceType, item);
                }
            }

            return serviceCollection;
        }

        public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
        {
            //serviceCollection.AddHttpService(lifeTime: 300, retryCount: 2, retryWait: 1);
            serviceCollection.AddSingleton<IMongoDbService, MongoDbService>();

            return serviceCollection;
        }
    }
}
