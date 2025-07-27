using System.Collections.Concurrent;
using System.Reflection;

namespace EventStore.Commands;

[Obsolete("Use CommandDispatcher instead")]
public static class CommandRegistration
{
    static readonly ConcurrentDictionary<Type, Type> CommandHandlerMap = new();

    public static void FromAssembly(Assembly assembly)
    {
        var resolvedCommandTypes = new HashSet<Type>();
        var commandTypes = assembly
            .GetTypes()
            .Where(x => typeof(ICommand).IsAssignableFrom(x) && x is { IsInterface: false, IsAbstract: false, IsClass: true });

        var commandHandlerTypes = assembly
            .GetTypes()
            .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(ICommandHandler<>)));
        
        foreach (var commandHandlerType in commandHandlerTypes)
        {
            var commandType = commandHandlerType.GetInterfaces().First().GetGenericArguments().First();
            resolvedCommandTypes.Add(commandType);
            CommandHandlerMap.TryAdd(commandType, commandHandlerType);
        }

        var missingCommandTypes = commandTypes.Except(resolvedCommandTypes);
        if (missingCommandTypes.Any())
        {
            throw new Exception();
        }
    }
    
    public static ICommandHandler<T> ResolveHandler<T>() where T : ICommand
    {
        return (ICommandHandler<T>)Activator.CreateInstance(CommandHandlerMap[typeof(T)])!;
    }
}