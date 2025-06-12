using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Agent.Functions;

/// <summary>
/// MyNumbers Emergency Services API plugin
/// Based on BICS MyNumbers Emergency Services API OpenAPI specification
/// </summary>
public class MyNumbersEmergencyServicesApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersEmergencyServicesApiPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersEmergencyServicesApiPlugin(HttpClient httpClient, ILogger<MyNumbersEmergencyServicesApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:MyNumbersEmergencyServices:BaseUrl"] ?? "https://mynumbers.bics.com";
    }

    [KernelFunction, Description("Configure emergency services")]
    public async Task<string> ConfigureEmergencyServicesAsync(
        [Description("Emergency services configuration as JSON")] string emergencyConfig)
    {
        try
        {
            _logger.LogInformation("Configuring emergency services");
            
            if (string.IsNullOrWhiteSpace(emergencyConfig))
            {
                throw new ArgumentException("Emergency configuration cannot be null or empty", nameof(emergencyConfig));
            }

            var content = new StringContent(emergencyConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/emergency-services/configure", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully configured emergency services");
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure emergency services");
            throw;
        }
    }

    [KernelFunction, Description("Get emergency services status")]
    public async Task<string> GetEmergencyServicesStatusAsync(
        [Description("Number to check emergency services for")] string number)
    {
        try
        {
            _logger.LogInformation("Getting emergency services status for number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/emergency-services/status/{number}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved emergency services status");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get emergency services status for number: {Number}", number);
            throw;
        }
    }

    [KernelFunction, Description("Update emergency location")]
    public async Task<string> UpdateEmergencyLocationAsync(
        [Description("Number to update location for")] string number,
        [Description("Location information as JSON")] string locationInfo)
    {
        try
        {
            _logger.LogInformation("Updating emergency location for number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            if (string.IsNullOrWhiteSpace(locationInfo))
            {
                throw new ArgumentException("Location information cannot be null or empty", nameof(locationInfo));
            }

            var content = new StringContent(locationInfo, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/v1/emergency-services/location/{number}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully updated emergency location");
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update emergency location for number: {Number}", number);
            throw;
        }
    }
}