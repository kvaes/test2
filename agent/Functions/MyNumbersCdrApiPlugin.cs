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
/// MyNumbers CDR (Call Detail Records) API plugin
/// Based on BICS MyNumbers CDR API OpenAPI specification
/// </summary>
public class MyNumbersCdrApiPlugin
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MyNumbersCdrApiPlugin> _logger;
    private readonly string _baseUrl;

    public MyNumbersCdrApiPlugin(HttpClient httpClient, ILogger<MyNumbersCdrApiPlugin> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _baseUrl = configuration["ApiSettings:MyNumbersCDR:BaseUrl"] ?? "https://mynumbers.bics.com";
    }

    [KernelFunction, Description("Get CDR service status")]
    public async Task<string> GetServiceStatusAsync()
    {
        try
        {
            _logger.LogInformation("Getting CDR service status");
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/cdr/status");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved CDR service status");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get CDR service status");
            throw;
        }
    }

    [KernelFunction, Description("Get call detail records")]
    public async Task<string> GetCallDetailRecordsAsync(
        [Description("Start date (ISO format)")] string startDate,
        [Description("End date (ISO format)")] string endDate,
        [Description("Number filter (optional)")] string? number = null,
        [Description("Page number (optional)")] int page = 1,
        [Description("Page size (optional)")] int pageSize = 100)
    {
        try
        {
            _logger.LogInformation("Getting call detail records from {StartDate} to {EndDate}", startDate, endDate);
            
            if (string.IsNullOrWhiteSpace(startDate) || string.IsNullOrWhiteSpace(endDate))
            {
                throw new ArgumentException("Start date and end date are required");
            }

            var queryParams = new List<string>
            {
                $"startDate={startDate}",
                $"endDate={endDate}",
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (!string.IsNullOrWhiteSpace(number))
                queryParams.Add($"number={number}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/cdr/records?{queryString}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully retrieved call detail records");
            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get call detail records");
            throw;
        }
    }

    [KernelFunction, Description("Generate CDR report")]
    public async Task<string> GenerateCdrReportAsync(
        [Description("Report configuration as JSON")] string reportConfig)
    {
        try
        {
            _logger.LogInformation("Generating CDR report");
            
            if (string.IsNullOrWhiteSpace(reportConfig))
            {
                throw new ArgumentException("Report configuration cannot be null or empty", nameof(reportConfig));
            }

            var content = new StringContent(reportConfig, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/cdr/reports", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully generated CDR report");
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate CDR report");
            throw;
        }
    }

    [KernelFunction, Description("Export CDR data")]
    public async Task<string> ExportCdrDataAsync(
        [Description("Export criteria as JSON")] string exportCriteria)
    {
        try
        {
            _logger.LogInformation("Exporting CDR data");
            
            if (string.IsNullOrWhiteSpace(exportCriteria))
            {
                throw new ArgumentException("Export criteria cannot be null or empty", nameof(exportCriteria));
            }

            var content = new StringContent(exportCriteria, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/cdr/export", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Successfully exported CDR data");
            return responseContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export CDR data");
            throw;
        }
    }
}