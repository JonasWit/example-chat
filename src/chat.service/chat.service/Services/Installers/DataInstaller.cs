using chat.service.Data;
using chat.service.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace chat.service.Services.Installers;

public class DataInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddDbContextFactory<ChatContext>(options =>
            options.UseNpgsql(webApplicationBuilder.Configuration.GetConnectionString("MainDb"),
                    serverAction =>
                    {
                        _ = serverAction.EnableRetryOnFailure(5);
                        _ = serverAction.CommandTimeout(20);
                    }));
        _ = webApplicationBuilder.Services.AddScoped<ChatRepository>();
    }
}