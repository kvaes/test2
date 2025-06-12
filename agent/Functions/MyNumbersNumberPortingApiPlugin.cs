using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Agent.Functions;

/// <summary>
/// MyNumbers Number Porting API plugin
/// Based on BICS MyNumbers Number Porting API OpenAPI specification
/// </summary>
public class MyNumbersNumberPortingApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersNumberPortingApiPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersNumberPortingApiPlugin(HttpClient httpClient, ILogger<MyNumbersNumberPortingApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:MyNumbersNumberPorting:BaseUrl"] ?? "https://mynumbers.bics.com";
    }

    [KernelFunction, Description("Submit number porting request")]
    public async Task<string> SubmitPortingRequestAsync(
        [Description("Porting request as JSON")] string portingRequest)
    {
        try
        {
            _logger.LogInformation("Submitting number porting request");
            
            if (string.IsNullOrWhiteSpace(portingRequest))
            {
                throw new ArgumentException("Porting request cannot be null or empty", nameof(portingRequest));
            }

            var content = new StringContent(portingRequest, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/number-porting/requests", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully submitted number porting request");
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit number porting request");
            throw;
        }
    }

    [KernelFunction, Description("Get porting request status")]
    public async Task<string> GetPortingStatusAsync(
        [Description("Porting request ID")] string requestId)
    {
        try
        {
            _logger.LogInformation("Getting porting status for request ID: {RequestId}", requestId);
            
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new ArgumentException("Request ID cannot be null or empty", nameof(requestId));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/number-porting/requests/{requestId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved porting status");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get porting status for request ID: {RequestId}", requestId);
            throw;
        }
    }

    [KernelFunction, Description("Update porting request")]
    public async Task<string> UpdatePortingRequestAsync(
        [Description("Porting request ID")] string requestId,
        [Description("Updated porting information as JSON")] string portingUpdate)
    {
        try
        {
            _logger.LogInformation("Updating porting request ID: {RequestId}", requestId);
            
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new ArgumentException("Request ID cannot be null or empty", nameof(requestId));
            }

            if (string.IsNullOrWhiteSpace(portingUpdate))
            {
                throw new ArgumentException("Porting update cannot be null or empty", nameof(portingUpdate));
            }

            var content = new StringContent(portingUpdate, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/v1/number-porting/requests/{requestId}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully updated porting request");
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update porting request ID: {RequestId}", requestId);
            throw;
        }
    }

    [KernelFunction, Description("Cancel porting request")]
    public async Task<string> CancelPortingRequestAsync(
        [Description("Porting request ID")] string requestId)
    {
        try
        {
            _logger.LogInformation("Cancelling porting request ID: {RequestId}", requestId);
            
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new ArgumentException("Request ID cannot be null or empty", nameof(requestId));
            }

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/number-porting/requests/{requestId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully cancelled porting request");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel porting request ID: {RequestId}", requestId);
            throw;
        }
    }

    [KernelFunction, Description("Get porting history")]
    public async Task<string> GetPortingHistoryAsync(
        [Description("Number to get porting history for")] string number)
    {
        try
        {
            _logger.LogInformation("Getting porting history for number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/number-porting/history/{number}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved porting history");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get porting history for number: {Number}", number);
            throw;
        }
    }
}