# API Plugins Documentation

This document provides detailed information about the API plugins available in the Agent project.

## Overview

The Agent project includes plugins for various BICS APIs, each exposing multiple endpoints as Semantic Kernel functions. All plugins are automatically loaded during application startup and are available for use within the agent's processing logic.

## Available Plugins

### 1. Connect API Plugin (`ConnectApi`)

**Base URL Configuration**: `ApiSettings:Connect:BaseUrl`
**Default URL**: `https://connect.bics.com`

#### Functions:
- `GetConnectionStatusAsync()` - Get connection status
- `CreateConnectionAsync(connectionConfig)` - Create a new connection
- `GetConnectionByIdAsync(connectionId)` - Get connection details by ID
- `UpdateConnectionAsync(connectionId, connectionConfig)` - Update an existing connection
- `DeleteConnectionAsync(connectionId)` - Delete a connection
- `ListConnectionsAsync(page?, pageSize?)` - List all connections with pagination
- `TestConnectionAsync(connectionId)` - Test connection connectivity

### 2. MyNumbers API Plugin (`MyNumbersApi`)

**Base URL Configuration**: `ApiSettings:MyNumbers:BaseUrl`
**Default URL**: `https://mynumbers.bics.com`

#### Functions:
- `GetServiceStatusAsync()` - Get MyNumbers service status
- `GetNumberInventoryAsync(countryCode?, numberType?, page?, pageSize?)` - Get number inventory
- `ReserveNumberAsync(number, reservationConfig)` - Reserve a number
- `ActivateNumberAsync(number, activationConfig)` - Activate a number
- `GetNumberDetailsAsync(number)` - Get number details
- `UpdateNumberConfigurationAsync(number, configurationUpdate)` - Update number configuration
- `DeactivateNumberAsync(number)` - Deactivate a number
- `GetNumberHistoryAsync(number, startDate?, endDate?)` - Get number history
- `SearchNumbersAsync(searchCriteria, page?, pageSize?)` - Search numbers by criteria

### 3. MyNumbers Address Management API Plugin (`MyNumbersAddressManagementApi`)

**Base URL Configuration**: `ApiSettings:MyNumbersAddressManagement:BaseUrl`
**Default URL**: `https://mynumbers.bics.com`

#### Functions:
- `GetServiceStatusAsync()` - Get address management service status
- `CreateAddressAsync(addressInfo)` - Create address record
- `GetAddressAsync(addressId)` - Get address by ID
- `UpdateAddressAsync(addressId, addressInfo)` - Update address information
- `DeleteAddressAsync(addressId)` - Delete address record
- `SearchAddressesAsync(searchCriteria, page?, pageSize?)` - Search addresses by criteria
- `ValidateAddressAsync(addressData)` - Validate address
- `GetAddressHistoryAsync(addressId, startDate?, endDate?)` - Get address history
- `BulkImportAddressesAsync(bulkAddressData)` - Bulk import addresses
- `ExportAddressesAsync(exportCriteria)` - Export addresses

### 4. MyNumbers CDR API Plugin (`MyNumbersCdrApi`)

**Base URL Configuration**: `ApiSettings:MyNumbersCDR:BaseUrl`
**Default URL**: `https://mynumbers.bics.com`

#### Functions:
- `GetServiceStatusAsync()` - Get CDR service status
- `GetCallDetailRecordsAsync(startDate, endDate, number?, page?, pageSize?)` - Get call detail records
- `GenerateCdrReportAsync(reportConfig)` - Generate CDR report
- `ExportCdrDataAsync(exportCriteria)` - Export CDR data

### 5. MyNumbers Disconnection API Plugin (`MyNumbersDisconnectionApi`)

**Base URL Configuration**: `ApiSettings:MyNumbersDisconnection:BaseUrl`
**Default URL**: `https://mynumbers.bics.com`

#### Functions:
- `RequestDisconnectionAsync(disconnectionRequest)` - Request number disconnection
- `GetDisconnectionStatusAsync(requestId)` - Get disconnection status
- `CancelDisconnectionAsync(requestId)` - Cancel disconnection request

### 6. MyNumbers Emergency Services API Plugin (`MyNumbersEmergencyServicesApi`)

**Base URL Configuration**: `ApiSettings:MyNumbersEmergencyServices:BaseUrl`
**Default URL**: `https://mynumbers.bics.com`

#### Functions:
- `ConfigureEmergencyServicesAsync(emergencyConfig)` - Configure emergency services
- `GetEmergencyServicesStatusAsync(number)` - Get emergency services status
- `UpdateEmergencyLocationAsync(number, locationInfo)` - Update emergency location

### 7. MyNumbers Number Porting API Plugin (`MyNumbersNumberPortingApi`)

**Base URL Configuration**: `ApiSettings:MyNumbersNumberPorting:BaseUrl`
**Default URL**: `https://mynumbers.bics.com`

#### Functions:
- `SubmitPortingRequestAsync(portingRequest)` - Submit number porting request
- `GetPortingStatusAsync(requestId)` - Get porting request status
- `UpdatePortingRequestAsync(requestId, portingUpdate)` - Update porting request
- `CancelPortingRequestAsync(requestId)` - Cancel porting request
- `GetPortingHistoryAsync(number)` - Get porting history

### 8. SMS API Plugin (`SmsApi`)

**Base URL Configuration**: `ApiSettings:SMS:BaseUrl`
**Default URL**: `https://sms.bics.com`

#### Functions:
- `GetServiceStatusAsync()` - Get SMS service status
- `SendSmsAsync(messageConfig)` - Send SMS message
- `SendBulkSmsAsync(bulkMessageConfig)` - Send bulk SMS messages
- `GetMessageStatusAsync(messageId)` - Get message status
- `GetDeliveryReportsAsync(messageId)` - Get message delivery reports
- `ScheduleSmsAsync(scheduledMessageConfig)` - Schedule SMS message
- `CancelScheduledMessageAsync(messageId)` - Cancel scheduled message
- `GetMessageHistoryAsync(startDate?, endDate?, status?, page?, pageSize?)` - Get message history
- `GetTemplatesAsync(category?, page?, pageSize?)` - Get SMS templates
- `CreateTemplateAsync(templateConfig)` - Create SMS template
- `GetAccountBalanceAsync()` - Get account balance

## Configuration

### URL Configuration

All API endpoints are configurable through the `appsettings.json` file:

```json
{
  "ApiSettings": {
    "Connect": {
      "BaseUrl": "https://connect.bics.com"
    },
    "MyNumbers": {
      "BaseUrl": "https://mynumbers.bics.com"
    },
    "MyNumbersAddressManagement": {
      "BaseUrl": "https://mynumbers.bics.com"
    },
    "MyNumbersCDR": {
      "BaseUrl": "https://mynumbers.bics.com"
    },
    "MyNumbersDisconnection": {
      "BaseUrl": "https://mynumbers.bics.com"
    },
    "MyNumbersEmergencyServices": {
      "BaseUrl": "https://mynumbers.bics.com"
    },
    "MyNumbersNumberPorting": {
      "BaseUrl": "https://mynumbers.bics.com"
    },
    "SMS": {
      "BaseUrl": "https://sms.bics.com"
    }
  }
}
```

### Environment Variables

You can also configure URLs using environment variables:

```bash
export ApiSettings__Connect__BaseUrl="https://connect-dev.bics.com"
export ApiSettings__SMS__BaseUrl="https://sms-dev.bics.com"
```

## Usage Examples

### Using Plugins in Semantic Kernel

```csharp
// Example of using the Connect API plugin
var result = await kernel.InvokeAsync("ConnectApi", "GetConnectionStatusAsync");

// Example with parameters
var connectionConfig = JsonSerializer.Serialize(new { 
    name = "My Connection", 
    type = "sip" 
});
var connection = await kernel.InvokeAsync("ConnectApi", "CreateConnectionAsync", 
    new KernelArguments { ["connectionConfig"] = connectionConfig });
```

### Function Parameters

Most functions accept JSON strings as parameters. Here are some examples:

#### Creating a Connection
```json
{
  "name": "My SIP Connection",
  "type": "sip",
  "configuration": {
    "host": "sip.example.com",
    "port": 5060,
    "protocol": "UDP"
  }
}
```

#### Sending an SMS
```json
{
  "to": "+1234567890",
  "from": "+0987654321",
  "message": "Hello from BICS SMS API",
  "type": "text"
}
```

#### Number Reservation
```json
{
  "duration": "30days",
  "purpose": "customer_assignment",
  "metadata": {
    "customer_id": "12345",
    "request_id": "req_789"
  }
}
```

## Error Handling

All plugin functions include comprehensive error handling:

- **Argument Validation**: Null/empty parameter checks
- **HTTP Error Handling**: Proper status code validation
- **Logging**: Detailed logging for debugging and monitoring
- **Exception Propagation**: Clean exception handling with context

## Authentication

The plugins are designed to work with the existing authentication mechanisms of the BICS APIs. Configure authentication headers, API keys, or other authentication methods through the HttpClient configuration in the dependency injection setup.

## Monitoring and Logging

All plugin operations are logged with appropriate log levels:

- **Information**: Successful operations and status updates
- **Warning**: Non-critical issues or deprecation notices
- **Error**: Failed operations with detailed error information
- **Debug**: Detailed execution flow (in development mode)

## Development and Testing

### Testing Individual Plugins

You can test individual plugins by creating instances with mock HttpClients:

```csharp
var mockHttpClient = new Mock<HttpClient>();
var mockLogger = new Mock<ILogger<ConnectApiPlugin>>();
var mockConfig = new Mock<IConfiguration>();

var plugin = new ConnectApiPlugin(mockHttpClient.Object, mockLogger.Object, mockConfig.Object);
```

### Adding New Functions

To add new functions to existing plugins:

1. Add the function method with `[KernelFunction]` and `[Description]` attributes
2. Include proper parameter validation and error handling
3. Add appropriate logging
4. Update this documentation

### Creating New Plugins

To create new API plugins:

1. Create a new class in the `Agent.Functions` namespace
2. Follow the existing plugin patterns for constructor injection
3. Implement functions with proper attributes and error handling
4. Register the plugin in `AgentService.LoadPluginsAsync()`
5. Add configuration settings to `appsettings.json`
6. Update this documentation

## Troubleshooting

### Common Issues

1. **Plugin Not Loading**: Check logger output for plugin loading errors
2. **Configuration Issues**: Verify `appsettings.json` structure and values
3. **HTTP Errors**: Check API endpoint URLs and authentication
4. **JSON Serialization**: Ensure parameter JSON is properly formatted

### Debug Mode

Enable debug logging in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Agent.Functions": "Debug"
    }
  }
}
```

This will provide detailed information about plugin operations and HTTP requests.