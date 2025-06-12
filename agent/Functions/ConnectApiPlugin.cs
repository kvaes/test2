using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.Text.Json;

namespace Agent.Functions;

/// <summary>
/// Connect API plugin for managing connections and communication services
/// Based on BICS Connect API OpenAPI specification
/// </summary>
public class ConnectApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConnectApiPlugin> _logger;
    private readonly string _baseUrl;

    public ConnectApiPlugin(HttpClient httpClient, ILogger<ConnectApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:Connect:BaseUrl"] ?? "https://connect.bics.com";
    }

    [KernelFunction, Description("Get connection status")]
    public async Task<string> GetConnectionStatusAsync()
    {
        try
        {
            _logger.LogInformation("Getting connection status from Connect API");
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/status");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved connection status");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get connection status");
            throw;
        }
    }

    [KernelFunction, Description("Create a new connection")]
    public async Task<string> CreateConnectionAsync(
        [Description("Connection configuration as JSON")] string connectionConfig)
    {
        try
        {
            _logger.LogInformation("Creating new connection via Connect API");
            
            if (string.IsNullOrWhiteSpace(connectionConfig))
            {
                throw new ArgumentException("Connection configuration cannot be null or empty", nameof(connectionConfig));
            }

            var content = new StringContent(connectionConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/connections", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully created connection");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create connection");
            throw;
        }
    }

    [KernelFunction, Description("Get connection details by ID")]
    public async Task<string> GetConnectionByIdAsync(
        [Description("Connection ID")] string connectionId)
    {
        try
        {
            _logger.LogInformation("Getting connection details for ID: {ConnectionId}", connectionId);
            
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/connections/{connectionId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved connection details");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get connection details for ID: {ConnectionId}", connectionId);
            throw;
        }
    }

    [KernelFunction, Description("Update an existing connection")]
    public async Task<string> UpdateConnectionAsync(
        [Description("Connection ID")] string connectionId,
        [Description("Updated connection configuration as JSON")] string connectionConfig)
    {
        try
        {
            _logger.LogInformation("Updating connection {ConnectionId} via Connect API", connectionId);
            
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));
            }

            if (string.IsNullOrWhiteSpace(connectionConfig))
            {
                throw new ArgumentException("Connection configuration cannot be null or empty", nameof(connectionConfig));
            }

            var content = new StringContent(connectionConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/v1/connections/{connectionId}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully updated connection");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update connection {ConnectionId}", connectionId);
            throw;
        }
    }

    [KernelFunction, Description("Delete a connection")]
    public async Task<string> DeleteConnectionAsync(
        [Description("Connection ID")] string connectionId)
    {
        try
        {
            _logger.LogInformation("Deleting connection {ConnectionId} via Connect API", connectionId);
            
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));
            }

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/connections/{connectionId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully deleted connection");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete connection {ConnectionId}", connectionId);
            throw;
        }
    }

    [KernelFunction, Description("List all connections")]
    public async Task<string> ListConnectionsAsync(
        [Description("Page number (optional)")] int page = 1,
        [Description("Page size (optional)")] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Listing connections from Connect API (page: {Page}, size: {PageSize})", page, pageSize);
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/connections?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved connections list");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to list connections");
            throw;
        }
    }

    [KernelFunction, Description("Test connection connectivity")]
    public async Task<string> TestConnectionAsync(
        [Description("Connection ID")] string connectionId)
    {
        try
        {
            _logger.LogInformation("Testing connectivity for connection {ConnectionId}", connectionId);
            
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));
            }

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/connections/{connectionId}/test", null);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully tested connection connectivity");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to test connection {ConnectionId}", connectionId);
            throw;
        }
    }
}