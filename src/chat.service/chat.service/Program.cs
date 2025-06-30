using chat.service.Data;
using chat.service.Endpoints;
using chat.service.Hubs;
using chat.service.Services;
using chat.service.Services.Utilities;

namespace chat.service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options =>
            options.AddPolicy("CorsPolicy", b => b
                .WithOrigins("http://localhost:5050")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            )
        );

        builder.InstallServices(typeof(IServiceInstaller).Assembly);
        builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
        var app = builder.Build();
        app.UseCors("CorsPolicy");
        app.MapChatEndpoints();

        app.MapHub<BotGeneratorHub>("/bot-hub")
            .RequireCors("CorsPolicy");

        // not for prod of course
        DbSetup.PrepareDatabase(app);
        app.Run();
    }
}