using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Agent.Functions;

/// <summary>
/// MyNumbers API plugin for number management services
/// Based on BICS MyNumbers API OpenAPI specification
/// </summary>
public class MyNumbersApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersApiPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersApiPlugin(HttpClient httpClient, ILogger<MyNumbersApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:MyNumbers:BaseUrl"] ?? "https://mynumbers.bics.com";
    }

    [KernelFunction, Description("Get MyNumbers service status")]
    public async Task<string> GetServiceStatusAsync()
    {
        try
        {
            _logger.LogInformation("Getting MyNumbers service status");
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/status");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved service status");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get service status");
            throw;
        }
    }

    [KernelFunction, Description("Get number inventory")]
    public async Task<string> GetNumberInventoryAsync(
        [Description("Country code (optional)")] string? countryCode = null,
        [Description("Number type (optional)")] string? numberType = null,
        [Description("Page number (optional)")] int page = 1,
        [Description("Page size (optional)")] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting number inventory");
            
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(countryCode))
                queryParams.Add($"countryCode={countryCode}");
            if (!string.IsNullOrWhiteSpace(numberType))
                queryParams.Add($"numberType={numberType}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");
            
            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/numbers?{queryString}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved number inventory");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get number inventory");
            throw;
        }
    }

    [KernelFunction, Description("Reserve a number")]
    public async Task<string> ReserveNumberAsync(
        [Description("Number to reserve")] string number,
        [Description("Reservation configuration as JSON")] string reservationConfig)
    {
        try
        {
            _logger.LogInformation("Reserving number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            if (string.IsNullOrWhiteSpace(reservationConfig))
            {
                throw new ArgumentException("Reservation configuration cannot be null or empty", nameof(reservationConfig));
            }

            var content = new StringContent(reservationConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/numbers/{number}/reserve", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully reserved number");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reserve number: {Number}", number);
            throw;
        }
    }

    [KernelFunction, Description("Activate a number")]
    public async Task<string> ActivateNumberAsync(
        [Description("Number to activate")] string number,
        [Description("Activation configuration as JSON")] string activationConfig)
    {
        try
        {
            _logger.LogInformation("Activating number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            if (string.IsNullOrWhiteSpace(activationConfig))
            {
                throw new ArgumentException("Activation configuration cannot be null or empty", nameof(activationConfig));
            }

            var content = new StringContent(activationConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/numbers/{number}/activate", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully activated number");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to activate number: {Number}", number);
            throw;
        }
    }

    [KernelFunction, Description("Get number details")]
    public async Task<string> GetNumberDetailsAsync(
        [Description("Number to get details for")] string number)
    {
        try
        {
            _logger.LogInformation("Getting details for number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/numbers/{number}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved number details");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get number details: {Number}", number);
            throw;
        }
    }

    [KernelFunction, Description("Update number configuration")]
    public async Task<string> UpdateNumberConfigurationAsync(
        [Description("Number to update")] string number,
        [Description("Configuration update as JSON")] string configurationUpdate)
    {
        try
        {
            _logger.LogInformation("Updating configuration for number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            if (string.IsNullOrWhiteSpace(configurationUpdate))
            {
                throw new ArgumentException("Configuration update cannot be null or empty", nameof(configurationUpdate));
            }

            var content = new StringContent(configurationUpdate, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/v1/numbers/{number}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully updated number configuration");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update number configuration: {Number}", number);
            throw;
        }
    }

    [KernelFunction, Description("Deactivate a number")]
    public async Task<string> DeactivateNumberAsync(
        [Description("Number to deactivate")] string number)
    {
        try
        {
            _logger.LogInformation("Deactivating number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/numbers/{number}/deactivate", null);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully deactivated number");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deactivate number: {Number}", number);
            throw;
        }
    }

    [KernelFunction, Description("Get number history")]
    public async Task<string> GetNumberHistoryAsync(
        [Description("Number to get history for")] string number,
        [Description("Start date (optional, ISO format)")] string? startDate = null,
        [Description("End date (optional, ISO format)")] string? endDate = null)
    {
        try
        {
            _logger.LogInformation("Getting history for number: {Number}", number);
            
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number cannot be null or empty", nameof(number));
            }

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(startDate))
                queryParams.Add($"startDate={startDate}");
            if (!string.IsNullOrWhiteSpace(endDate))
                queryParams.Add($"endDate={endDate}");
            
            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/numbers/{number}/history{queryString}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved number history");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get number history: {Number}", number);
            throw;
        }
    }

    [KernelFunction, Description("Search numbers by criteria")]
    public async Task<string> SearchNumbersAsync(
        [Description("Search criteria as JSON")] string searchCriteria,
        [Description("Page number (optional)")] int page = 1,
        [Description("Page size (optional)")] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Searching numbers with criteria");
            
            if (string.IsNullOrWhiteSpace(searchCriteria))
            {
                throw new ArgumentException("Search criteria cannot be null or empty", nameof(searchCriteria));
            }

            var content = new StringContent(searchCriteria, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/numbers/search?page={page}&pageSize={pageSize}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully searched numbers");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search numbers");
            throw;
        }
    }
}