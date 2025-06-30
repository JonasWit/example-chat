using System.Reflection;

namespace chat.service.Services.Utilities;

public static class DIUtilities
{
    public static WebApplicationBuilder InstallServices(this WebApplicationBuilder webApplicationBuilder,
        params Assembly[] assemblies)
    {
        var serviceInstallers = assemblies
            .SelectMany(a => a.DefinedTypes)
            .Where(IsAssignableToType<IServiceInstaller>)
            .Select(Activator.CreateInstance)
            .Cast<IServiceInstaller>();

        foreach (var serviceInstaller in serviceInstallers) serviceInstaller.Install(webApplicationBuilder);

        return webApplicationBuilder;

        static bool IsAssignableToType<T>(TypeInfo typeInfo)
        {
            return typeof(T).IsAssignableFrom(typeInfo) &&
                   !typeInfo.IsInterface &&
                   !typeInfo.IsAbstract;
        }
    }
}