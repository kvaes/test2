using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Agent.Functions;

/// <summary>
/// MyNumbers Disconnection API plugin
/// Based on BICS MyNumbers Disconnection API OpenAPI specification
/// </summary>
public class MyNumbersDisconnectionApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersDisconnectionApiPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersDisconnectionApiPlugin(HttpClient httpClient, ILogger<MyNumbersDisconnectionApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:MyNumbersDisconnection:BaseUrl"] ?? "https://mynumbers.bics.com";
    }

    [KernelFunction, Description("Request number disconnection")]
    public async Task<string> RequestDisconnectionAsync(
        [Description("Disconnection request as JSON")] string disconnectionRequest)
    {
        try
        {
            _logger.LogInformation("Requesting number disconnection");
            
            if (string.IsNullOrWhiteSpace(disconnectionRequest))
            {
                throw new ArgumentException("Disconnection request cannot be null or empty", nameof(disconnectionRequest));
            }

            var content = new StringContent(disconnectionRequest, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/disconnection/requests", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully requested number disconnection");
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to request number disconnection");
            throw;
        }
    }

    [KernelFunction, Description("Get disconnection status")]
    public async Task<string> GetDisconnectionStatusAsync(
        [Description("Disconnection request ID")] string requestId)
    {
        try
        {
            _logger.LogInformation("Getting disconnection status for request ID: {RequestId}", requestId);
            
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new ArgumentException("Request ID cannot be null or empty", nameof(requestId));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/disconnection/requests/{requestId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved disconnection status");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get disconnection status for request ID: {RequestId}", requestId);
            throw;
        }
    }

    [KernelFunction, Description("Cancel disconnection request")]
    public async Task<string> CancelDisconnectionAsync(
        [Description("Disconnection request ID")] string requestId)
    {
        try
        {
            _logger.LogInformation("Cancelling disconnection request ID: {RequestId}", requestId);
            
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new ArgumentException("Request ID cannot be null or empty", nameof(requestId));
            }

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/disconnection/requests/{requestId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully cancelled disconnection request");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel disconnection request ID: {RequestId}", requestId);
            throw;
        }
    }
}