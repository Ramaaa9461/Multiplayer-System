using System.Collections.Generic;

public static class ServiceProvider
{
    private static readonly Dictionary<System.Type, object> services = new Dictionary<System.Type, object>();

    public static void RegisterService<T>(T service)
    {
        System.Type type = typeof(T);
        if (!services.ContainsKey(type))
        {
            services[type] = service;
        }
    }

    public static T GetService<T>()
    {
        System.Type type = typeof(T);
        if (services.TryGetValue(type, out object service))
        {
            return (T)service;
        }
        throw new KeyNotFoundException($"Service of type {type} not found.");
    }
}