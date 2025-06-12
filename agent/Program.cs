using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Serilog;
using System.Text.Json;

namespace Agent;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args).Build();
            
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting Agent application");

            await host.RunAsync();
            
            logger.LogInformation("Agent application stopped");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                
                // Add Semantic Kernel
                services.AddKernel();
                
                // Add HttpClient
                services.AddHttpClient();
                
                // Add custom services
                services.AddSingleton<IAgentService, AgentService>();
                services.AddHostedService<AgentBackgroundService>();
            });
}