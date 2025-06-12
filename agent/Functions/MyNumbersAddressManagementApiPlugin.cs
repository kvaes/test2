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
/// MyNumbers Address Management API plugin
/// Based on BICS MyNumbers Address Management API OpenAPI specification
/// </summary>
public class MyNumbersAddressManagementApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersAddressManagementApiPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersAddressManagementApiPlugin(HttpClient httpClient, ILogger<MyNumbersAddressManagementApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:MyNumbersAddressManagement:BaseUrl"] ?? "https://mynumbers.bics.com";
    }

    [KernelFunction, Description("Get address management service status")]
    public async Task<string> GetServiceStatusAsync()
    {
        try
        {
            _logger.LogInformation("Getting address management service status");
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/address-management/status");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved address management service status");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get address management service status");
            throw;
        }
    }

    [KernelFunction, Description("Create address record")]
    public async Task<string> CreateAddressAsync(
        [Description("Address information as JSON")] string addressInfo)
    {
        try
        {
            _logger.LogInformation("Creating address record");
            
            if (string.IsNullOrWhiteSpace(addressInfo))
            {
                throw new ArgumentException("Address information cannot be null or empty", nameof(addressInfo));
            }

            var content = new StringContent(addressInfo, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/address-management/addresses", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully created address record");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create address record");
            throw;
        }
    }

    [KernelFunction, Description("Get address by ID")]
    public async Task<string> GetAddressAsync(
        [Description("Address ID")] string addressId)
    {
        try
        {
            _logger.LogInformation("Getting address details for ID: {AddressId}", addressId);
            
            if (string.IsNullOrWhiteSpace(addressId))
            {
                throw new ArgumentException("Address ID cannot be null or empty", nameof(addressId));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/address-management/addresses/{addressId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved address details");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get address details for ID: {AddressId}", addressId);
            throw;
        }
    }

    [KernelFunction, Description("Update address information")]
    public async Task<string> UpdateAddressAsync(
        [Description("Address ID")] string addressId,
        [Description("Updated address information as JSON")] string addressInfo)
    {
        try
        {
            _logger.LogInformation("Updating address {AddressId}", addressId);
            
            if (string.IsNullOrWhiteSpace(addressId))
            {
                throw new ArgumentException("Address ID cannot be null or empty", nameof(addressId));
            }

            if (string.IsNullOrWhiteSpace(addressInfo))
            {
                throw new ArgumentException("Address information cannot be null or empty", nameof(addressInfo));
            }

            var content = new StringContent(addressInfo, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/api/v1/address-management/addresses/{addressId}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully updated address");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update address {AddressId}", addressId);
            throw;
        }
    }

    [KernelFunction, Description("Delete address record")]
    public async Task<string> DeleteAddressAsync(
        [Description("Address ID")] string addressId)
    {
        try
        {
            _logger.LogInformation("Deleting address {AddressId}", addressId);
            
            if (string.IsNullOrWhiteSpace(addressId))
            {
                throw new ArgumentException("Address ID cannot be null or empty", nameof(addressId));
            }

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/address-management/addresses/{addressId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully deleted address");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete address {AddressId}", addressId);
            throw;
        }
    }

    [KernelFunction, Description("Search addresses by criteria")]
    public async Task<string> SearchAddressesAsync(
        [Description("Search criteria as JSON")] string searchCriteria,
        [Description("Page number (optional)")] int page = 1,
        [Description("Page size (optional)")] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Searching addresses with criteria");
            
            if (string.IsNullOrWhiteSpace(searchCriteria))
            {
                throw new ArgumentException("Search criteria cannot be null or empty", nameof(searchCriteria));
            }

            var content = new StringContent(searchCriteria, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/address-management/addresses/search?page={page}&pageSize={pageSize}", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully searched addresses");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search addresses");
            throw;
        }
    }

    [KernelFunction, Description("Validate address")]
    public async Task<string> ValidateAddressAsync(
        [Description("Address data to validate as JSON")] string addressData)
    {
        try
        {
            _logger.LogInformation("Validating address");
            
            if (string.IsNullOrWhiteSpace(addressData))
            {
                throw new ArgumentException("Address data cannot be null or empty", nameof(addressData));
            }

            var content = new StringContent(addressData, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/address-management/addresses/validate", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully validated address");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate address");
            throw;
        }
    }

    [KernelFunction, Description("Get address history")]
    public async Task<string> GetAddressHistoryAsync(
        [Description("Address ID")] string addressId,
        [Description("Start date (optional, ISO format)")] string? startDate = null,
        [Description("End date (optional, ISO format)")] string? endDate = null)
    {
        try
        {
            _logger.LogInformation("Getting address history for ID: {AddressId}", addressId);
            
            if (string.IsNullOrWhiteSpace(addressId))
            {
                throw new ArgumentException("Address ID cannot be null or empty", nameof(addressId));
            }

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(startDate))
                queryParams.Add($"startDate={startDate}");
            if (!string.IsNullOrWhiteSpace(endDate))
                queryParams.Add($"endDate={endDate}");
            
            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/address-management/addresses/{addressId}/history{queryString}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved address history");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get address history for ID: {AddressId}", addressId);
            throw;
        }
    }

    [KernelFunction, Description("Bulk import addresses")]
    public async Task<string> BulkImportAddressesAsync(
        [Description("Bulk address data as JSON")] string bulkAddressData)
    {
        try
        {
            _logger.LogInformation("Bulk importing addresses");
            
            if (string.IsNullOrWhiteSpace(bulkAddressData))
            {
                throw new ArgumentException("Bulk address data cannot be null or empty", nameof(bulkAddressData));
            }

            var content = new StringContent(bulkAddressData, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/address-management/addresses/bulk-import", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully bulk imported addresses");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bulk import addresses");
            throw;
        }
    }

    [KernelFunction, Description("Export addresses")]
    public async Task<string> ExportAddressesAsync(
        [Description("Export criteria as JSON")] string exportCriteria)
    {
        try
        {
            _logger.LogInformation("Exporting addresses");
            
            if (string.IsNullOrWhiteSpace(exportCriteria))
            {
                throw new ArgumentException("Export criteria cannot be null or empty", nameof(exportCriteria));
            }

            var content = new StringContent(exportCriteria, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/address-management/addresses/export", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully exported addresses");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export addresses");
            throw;
        }
    }
}