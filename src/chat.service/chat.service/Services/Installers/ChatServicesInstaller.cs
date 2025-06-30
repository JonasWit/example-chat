namespace chat.service.Services.Installers;

public class ChatServicesInstaller  : IServiceInstaller
{
    public void Install(WebApplicationBuilder webApplicationBuilder)
    {
        _ = webApplicationBuilder.Services.AddKeyedTransient<IDummyChatService, Dummy2>("1");
        _ = webApplicationBuilder.Services.AddKeyedTransient<IDummyChatService, Dummy2>("2");   
        _ = webApplicationBuilder.Services.AddTransient<ChatProvider>(); 
    }
}