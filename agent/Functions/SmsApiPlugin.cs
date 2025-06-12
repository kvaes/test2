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
/// SMS API plugin for messaging services
/// Based on BICS SMS API OpenAPI specification
/// </summary>
public class SmsApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SmsApiPlugin> _logger;
    private readonly string _baseUrl;

    public SmsApiPlugin(HttpClient httpClient, ILogger<SmsApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:SMS:BaseUrl"] ?? "https://sms.bics.com";
    }

    [KernelFunction, Description("Get SMS service status")]
    public async Task<string> GetServiceStatusAsync()
    {
        try
        {
            _logger.LogInformation("Getting SMS service status");
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/status");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved SMS service status");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get SMS service status");
            throw;
        }
    }

    [KernelFunction, Description("Send SMS message")]
    public async Task<string> SendSmsAsync(
        [Description("SMS message configuration as JSON")] string messageConfig)
    {
        try
        {
            _logger.LogInformation("Sending SMS message");
            
            if (string.IsNullOrWhiteSpace(messageConfig))
            {
                throw new ArgumentException("Message configuration cannot be null or empty", nameof(messageConfig));
            }

            var content = new StringContent(messageConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/messages", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully sent SMS message");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send SMS message");
            throw;
        }
    }

    [KernelFunction, Description("Send bulk SMS messages")]
    public async Task<string> SendBulkSmsAsync(
        [Description("Bulk SMS configuration as JSON")] string bulkMessageConfig)
    {
        try
        {
            _logger.LogInformation("Sending bulk SMS messages");
            
            if (string.IsNullOrWhiteSpace(bulkMessageConfig))
            {
                throw new ArgumentException("Bulk message configuration cannot be null or empty", nameof(bulkMessageConfig));
            }

            var content = new StringContent(bulkMessageConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/messages/bulk", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully sent bulk SMS messages");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send bulk SMS messages");
            throw;
        }
    }

    [KernelFunction, Description("Get message status")]
    public async Task<string> GetMessageStatusAsync(
        [Description("Message ID")] string messageId)
    {
        try
        {
            _logger.LogInformation("Getting message status for ID: {MessageId}", messageId);
            
            if (string.IsNullOrWhiteSpace(messageId))
            {
                throw new ArgumentException("Message ID cannot be null or empty", nameof(messageId));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/messages/{messageId}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved message status");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get message status for ID: {MessageId}", messageId);
            throw;
        }
    }

    [KernelFunction, Description("Get message delivery reports")]
    public async Task<string> GetDeliveryReportsAsync(
        [Description("Message ID")] string messageId)
    {
        try
        {
            _logger.LogInformation("Getting delivery reports for message ID: {MessageId}", messageId);
            
            if (string.IsNullOrWhiteSpace(messageId))
            {
                throw new ArgumentException("Message ID cannot be null or empty", nameof(messageId));
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/messages/{messageId}/delivery-reports");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved delivery reports");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get delivery reports for message ID: {MessageId}", messageId);
            throw;
        }
    }

    [KernelFunction, Description("Schedule SMS message")]
    public async Task<string> ScheduleSmsAsync(
        [Description("Scheduled message configuration as JSON")] string scheduledMessageConfig)
    {
        try
        {
            _logger.LogInformation("Scheduling SMS message");
            
            if (string.IsNullOrWhiteSpace(scheduledMessageConfig))
            {
                throw new ArgumentException("Scheduled message configuration cannot be null or empty", nameof(scheduledMessageConfig));
            }

            var content = new StringContent(scheduledMessageConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/messages/schedule", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully scheduled SMS message");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule SMS message");
            throw;
        }
    }

    [KernelFunction, Description("Cancel scheduled message")]
    public async Task<string> CancelScheduledMessageAsync(
        [Description("Scheduled message ID")] string messageId)
    {
        try
        {
            _logger.LogInformation("Cancelling scheduled message ID: {MessageId}", messageId);
            
            if (string.IsNullOrWhiteSpace(messageId))
            {
                throw new ArgumentException("Message ID cannot be null or empty", nameof(messageId));
            }

            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/v1/messages/{messageId}/schedule");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully cancelled scheduled message");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel scheduled message ID: {MessageId}", messageId);
            throw;
        }
    }

    [KernelFunction, Description("Get message history")]
    public async Task<string> GetMessageHistoryAsync(
        [Description("Start date (optional, ISO format)")] string? startDate = null,
        [Description("End date (optional, ISO format)")] string? endDate = null,
        [Description("Status filter (optional)")] string? status = null,
        [Description("Page number (optional)")] int page = 1,
        [Description("Page size (optional)")] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting message history");
            
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(startDate))
                queryParams.Add($"startDate={startDate}");
            if (!string.IsNullOrWhiteSpace(endDate))
                queryParams.Add($"endDate={endDate}");
            if (!string.IsNullOrWhiteSpace(status))
                queryParams.Add($"status={status}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");
            
            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/messages/history?{queryString}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved message history");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get message history");
            throw;
        }
    }

    [KernelFunction, Description("Get SMS templates")]
    public async Task<string> GetTemplatesAsync(
        [Description("Template category (optional)")] string? category = null,
        [Description("Page number (optional)")] int page = 1,
        [Description("Page size (optional)")] int pageSize = 10)
    {
        try
        {
            _logger.LogInformation("Getting SMS templates");
            
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(category))
                queryParams.Add($"category={category}");
            queryParams.Add($"page={page}");
            queryParams.Add($"pageSize={pageSize}");
            
            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/templates?{queryString}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved SMS templates");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get SMS templates");
            throw;
        }
    }

    [KernelFunction, Description("Create SMS template")]
    public async Task<string> CreateTemplateAsync(
        [Description("Template configuration as JSON")] string templateConfig)
    {
        try
        {
            _logger.LogInformation("Creating SMS template");
            
            if (string.IsNullOrWhiteSpace(templateConfig))
            {
                throw new ArgumentException("Template configuration cannot be null or empty", nameof(templateConfig));
            }

            var content = new StringContent(templateConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/templates", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully created SMS template");
            
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create SMS template");
            throw;
        }
    }

    [KernelFunction, Description("Get account balance")]
    public async Task<string> GetAccountBalanceAsync()
    {
        try
        {
            _logger.LogInformation("Getting account balance");
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/account/balance");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved account balance");
            
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get account balance");
            throw;
        }
    }
}