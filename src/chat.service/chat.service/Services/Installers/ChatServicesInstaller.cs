namespace chat.service.Services.Installers;

public class ChatServicesInstaller  : IServiceInstaller
{
    public void Install(WebApplicationBuilder webApplicationBuilder)
    {
        _ = webApplicationBuilder.Services.AddTransient<IDummyChatService, DummyChatService>();
    }
}