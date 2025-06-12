# Configuration Guide

This guide covers all configuration options available in the Agent project.

## Configuration Sources

The Agent application uses the standard .NET configuration system with the following hierarchy (later sources override earlier ones):

1. `appsettings.json` - Base configuration
2. `appsettings.{Environment}.json` - Environment-specific configuration
3. Environment variables
4. Command line arguments
5. User secrets (development only)

## Core Configuration Files

### appsettings.json

The main configuration file containing default settings:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  },
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

### appsettings.Development.json

Development-specific configuration with more verbose logging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Agent.Functions": "Debug"
    }
  },
  "ApiSettings": {
    "Connect": {
      "BaseUrl": "https://connect-dev.bics.com"
    },
    "MyNumbers": {
      "BaseUrl": "https://mynumbers-dev.bics.com"
    },
    "SMS": {
      "BaseUrl": "https://sms-dev.bics.com"
    }
  }
}
```

### appsettings.Production.json

Production-specific configuration (create as needed):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Agent": "Information"
    }
  },
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/agent/agent-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

## Configuration Sections

### Logging Configuration

The application uses both Microsoft.Extensions.Logging and Serilog for comprehensive logging.

#### Microsoft.Extensions.Logging
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "Agent": "Debug",
      "Agent.Functions": "Information"
    }
  }
}
```

**Log Levels:**
- `Trace` - Most verbose, internal flow information
- `Debug` - Development information, debugging details
- `Information` - General application flow, business events
- `Warning` - Potentially harmful situations
- `Error` - Error events, application continues
- `Critical` - Critical failures, application may terminate

#### Serilog Configuration
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/agent-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
  }
}
```

### API Settings Configuration

All API endpoints are configurable through the `ApiSettings` section:

```json
{
  "ApiSettings": {
    "Connect": {
      "BaseUrl": "https://connect.bics.com",
      "Timeout": "00:00:30",
      "RetryPolicy": {
        "MaxRetries": 3,
        "BaseDelay": "00:00:01"
      },
      "Authentication": {
        "Type": "ApiKey",
        "HeaderName": "X-API-Key",
        "Value": "${CONNECT_API_KEY}"
      }
    },
    "MyNumbers": {
      "BaseUrl": "https://mynumbers.bics.com",
      "Timeout": "00:01:00",
      "RetryPolicy": {
        "MaxRetries": 3,
        "BaseDelay": "00:00:02"
      }
    },
    "SMS": {
      "BaseUrl": "https://sms.bics.com",
      "Timeout": "00:00:15",
      "RateLimit": {
        "RequestsPerMinute": 100
      }
    }
  }
}
```

## Environment Variables

Environment variables can override any configuration setting using the double underscore (`__`) separator:

### Basic Examples
```bash
# Override API base URLs
export ApiSettings__Connect__BaseUrl="https://connect-staging.bics.com"
export ApiSettings__SMS__BaseUrl="https://sms-staging.bics.com"

# Override logging levels
export Logging__LogLevel__Default="Debug"
export Serilog__MinimumLevel__Default="Debug"

# Set environment
export ASPNETCORE_ENVIRONMENT="Development"
export DOTNET_ENVIRONMENT="Development"
```

### Authentication Configuration
```bash
# API Keys
export ApiSettings__Connect__Authentication__Value="your-connect-api-key"
export ApiSettings__SMS__Authentication__Value="your-sms-api-key"

# OAuth Configuration
export ApiSettings__MyNumbers__Authentication__ClientId="your-client-id"
export ApiSettings__MyNumbers__Authentication__ClientSecret="your-client-secret"
```

### Container Environment Variables
```bash
# For Docker deployments
export ApiSettings__Connect__BaseUrl="http://connect-api-service:8080"
export ApiSettings__MyNumbers__BaseUrl="http://mynumbers-api-service:8080"
export ApiSettings__SMS__BaseUrl="http://sms-api-service:8080"
```

## Advanced Configuration

### HTTP Client Configuration

Configure HTTP client behavior for API plugins:

```json
{
  "HttpClient": {
    "DefaultTimeout": "00:01:00",
    "MaxConnectionsPerServer": 10,
    "PooledConnectionLifetime": "00:05:00",
    "Headers": {
      "User-Agent": "BICS-Agent/1.0",
      "Accept": "application/json"
    }
  }
}
```

### Retry Policies

Configure retry behavior for resilient API calls:

```json
{
  "RetryPolicies": {
    "Default": {
      "MaxRetries": 3,
      "BaseDelay": "00:00:01",
      "MaxDelay": "00:00:30",
      "BackoffType": "Exponential"
    },
    "SMS": {
      "MaxRetries": 5,
      "BaseDelay": "00:00:02",
      "MaxDelay": "00:01:00",
      "BackoffType": "Linear"
    }
  }
}
```

### Rate Limiting

Configure rate limiting for API calls:

```json
{
  "RateLimiting": {
    "Policies": {
      "SMS": {
        "RequestsPerMinute": 100,
        "BurstLimit": 20
      },
      "Connect": {
        "RequestsPerMinute": 200,
        "BurstLimit": 50
      }
    }
  }
}
```

### Circuit Breaker

Configure circuit breaker patterns:

```json
{
  "CircuitBreaker": {
    "FailureThreshold": 5,
    "SamplingDuration": "00:01:00",
    "MinimumThroughput": 10,
    "DurationOfBreak": "00:00:30"
  }
}
```

## Security Configuration

### API Authentication

Configure authentication for different APIs:

```json
{
  "Authentication": {
    "Connect": {
      "Type": "ApiKey",
      "HeaderName": "X-API-Key",
      "SecretName": "connect-api-key"
    },
    "MyNumbers": {
      "Type": "OAuth2",
      "TokenEndpoint": "https://auth.bics.com/oauth/token",
      "ClientId": "your-client-id",
      "SecretName": "mynumbers-client-secret",
      "Scopes": ["numbers:read", "numbers:write"]
    },
    "SMS": {
      "Type": "Bearer",
      "SecretName": "sms-bearer-token"
    }
  }
}
```

### TLS/SSL Configuration

Configure TLS settings for secure communication:

```json
{
  "HttpClient": {
    "TlsSettings": {
      "MinimumTlsVersion": "TLSv12",
      "CertificateValidation": "ChainValidation",
      "AllowSelfSignedCertificates": false
    }
  }
}
```

## User Secrets (Development)

For development, use user secrets to store sensitive information:

```bash
# Initialize user secrets
dotnet user-secrets init

# Set API keys
dotnet user-secrets set "ApiSettings:Connect:Authentication:Value" "your-connect-api-key"
dotnet user-secrets set "ApiSettings:SMS:Authentication:Value" "your-sms-api-key"
```

## Configuration Validation

The application validates configuration on startup. Common validation rules:

- **Required URLs**: All API base URLs must be valid HTTP/HTTPS URLs
- **Timeout Values**: Must be positive TimeSpan values
- **Retry Settings**: Retry counts must be non-negative integers
- **Log Levels**: Must be valid LogLevel enum values

### Custom Configuration Validation

Add custom validation in `Program.cs`:

```csharp
services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
services.AddSingleton<IValidateOptions<ApiSettings>, ApiSettingsValidation>();
```

## Configuration Best Practices

### 1. Environment-Specific Settings
- Use `appsettings.{Environment}.json` for environment-specific overrides
- Never store secrets in configuration files
- Use environment variables or secrets management for sensitive data

### 2. Configuration Structure
- Group related settings in logical sections
- Use consistent naming conventions
- Document configuration options

### 3. Security
- Use secure secret storage (Azure Key Vault, Kubernetes secrets, etc.)
- Rotate API keys and credentials regularly
- Use least-privilege access principles

### 4. Monitoring
- Configure appropriate log levels for each environment
- Use structured logging for better searchability
- Monitor configuration changes

## Troubleshooting Configuration

### Common Issues

1. **Missing Configuration**
   ```bash
   # Check if configuration is loaded
   dotnet run --environment Development --verbosity diagnostic
   ```

2. **Environment Variables Not Working**
   ```bash
   # Verify environment variable format
   echo $ApiSettings__Connect__BaseUrl
   ```

3. **Log Level Issues**
   ```json
   // Enable debug logging for configuration
   {
     "Logging": {
       "LogLevel": {
         "Microsoft.Extensions.Configuration": "Debug"
       }
     }
   }
   ```

### Configuration Debugging

Enable configuration debugging:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.Extensions.Configuration": "Debug",
      "Microsoft.Extensions.Options": "Debug"
    }
  }
}
```

This will log configuration loading and binding information.

## Configuration in Different Environments

### Development
- Verbose logging enabled
- Development API endpoints
- Local file logging
- User secrets for sensitive data

### Staging
- Moderate logging
- Staging API endpoints
- Centralized logging
- Environment variables for secrets

### Production
- Minimal logging (Warning and above)
- Production API endpoints
- Structured logging to centralized system
- Secure secret management
- Performance monitoring enabled

## References

- [.NET Configuration Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Serilog Configuration](https://github.com/serilog/serilog-settings-configuration)
- [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)