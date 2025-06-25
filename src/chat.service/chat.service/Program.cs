using chat.service.Data;
using chat.service.Endpoints;
using chat.service.Services;
using chat.service.Services.Utilities;

namespace chat.service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(p => 
                p.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
        
        builder.InstallServices(typeof(IServiceInstaller).Assembly);
        builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
        var app = builder.Build();
        app.UseCors();
        app.MapChatEndpoints();
        
        // not for prod of course
        DbSetup.PrepareDatabase(app); 
        app.Run();
    }
}