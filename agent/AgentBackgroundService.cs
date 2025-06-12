using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Agent;

public class AgentBackgroundService : BackgroundService
{
    private readonly ILogger<AgentBackgroundService> _logger;
    private readonly IAgentService _agentService;

    public AgentBackgroundService(
        ILogger<AgentBackgroundService> logger,
        IAgentService agentService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _agentService = agentService ?? throw new ArgumentNullException(nameof(agentService));
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Agent Background Service starting");
            await _agentService.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Agent Background Service");
            throw;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Agent Background Service executing");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                // Main execution loop
                // This is where the agent will listen for and process requests
                
                await Task.Delay(1000, stoppingToken); // Placeholder delay
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Agent Background Service execution cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Agent Background Service execution");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Agent Background Service stopping");
            await _agentService.ShutdownAsync();
            await base.StopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping Agent Background Service");
            throw;
        }
    }
}