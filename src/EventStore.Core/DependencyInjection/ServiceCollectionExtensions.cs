using Microsoft.Extensions.DependencyInjection;

namespace EventStore.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLazy<TInterface, TConcrete>(this IServiceCollection serviceCollection) where TConcrete : class, TInterface where TInterface : class
    {
        serviceCollection.AddSingleton<TInterface, TConcrete>();
        serviceCollection.AddSingleton<Lazy<TInterface>>(x => new Lazy<TInterface>(x.GetRequiredService<TInterface>));
        return serviceCollection;
    }
}