using chat.service.Hubs;

namespace chat.service.Services.Installers;

public class SignalRInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder webApplicationBuilder)
    {
        _ = webApplicationBuilder.Services.AddSingleton<SignalRCache>();
        _ = webApplicationBuilder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = false;
            options.DisableImplicitFromServicesParameters = true;
        });
    }
}