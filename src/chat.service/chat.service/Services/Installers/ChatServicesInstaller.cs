namespace chat.service.Services.Installers;

public class ChatServicesInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder webApplicationBuilder)
    {
        _ = webApplicationBuilder.Services.AddSingleton<ChatBotStreamingService>();
        _ = webApplicationBuilder.Services.AddTransient<ChatProvider>();
    }
}