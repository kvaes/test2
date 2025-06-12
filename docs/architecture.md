# Project Architecture

This document provides an overview of the Agent project architecture, design patterns, and technical decisions.

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Agent Application                         │
├─────────────────────────────────────────────────────────────┤
│  Program.cs (Entry Point)                                  │
│  ├── Host Configuration                                    │
│  ├── Dependency Injection Setup                           │
│  ├── Logging Configuration (Serilog)                      │
│  └── Background Service Registration                      │
├─────────────────────────────────────────────────────────────┤
│  AgentBackgroundService                                    │
│  ├── Service Lifecycle Management                         │
│  ├── Background Task Execution                            │
│  └── Graceful Shutdown Handling                          │
├─────────────────────────────────────────────────────────────┤
│  AgentService (Core Business Logic)                       │
│  ├── Plugin Management                                    │
│  ├── Request Processing                                   │
│  ├── Error Handling                                       │
│  └── Semantic Kernel Integration                          │
├─────────────────────────────────────────────────────────────┤
│  Functions/ (API Plugins)                                 │
│  ├── ConnectApiPlugin                                     │
│  ├── MyNumbersApiPlugin                                   │
│  ├── MyNumbersAddressManagementApiPlugin                  │
│  ├── MyNumbersCdrApiPlugin                                │
│  ├── MyNumbersDisconnectionApiPlugin                      │
│  ├── MyNumbersEmergencyServicesApiPlugin                  │
│  ├── MyNumbersNumberPortingApiPlugin                      │
│  └── SmsApiPlugin                                         │
├─────────────────────────────────────────────────────────────┤
│  Processes/ (Business Process Definitions)                │
│  └── (Future process implementations)                     │
├─────────────────────────────────────────────────────────────┤
│  Steps/ (Reusable Process Steps)                          │
│  └── (Future step implementations)                        │
└─────────────────────────────────────────────────────────────┘
```

## Design Patterns

### 1. Dependency Injection Pattern

The application uses the built-in .NET dependency injection container for managing dependencies:

```csharp
services.AddKernel();                    // Semantic Kernel
services.AddHttpClient();               // HTTP client factory
services.AddSingleton<IAgentService, AgentService>();
services.AddHostedService<AgentBackgroundService>();
```

**Benefits:**
- Loose coupling between components
- Easy testing with mock dependencies
- Centralized configuration management
- Automatic dependency lifecycle management

### 2. Plugin Pattern

API integrations are implemented as plugins that integrate with Semantic Kernel:

```csharp
[KernelFunction, Description("Get connection status")]
public async Task<string> GetConnectionStatusAsync()
{
    // Implementation
}
```

**Benefits:**
- Modular architecture
- Easy to add new API integrations
- Consistent error handling across plugins
- Semantic Kernel function discovery

### 3. Background Service Pattern

Uses .NET's `BackgroundService` for long-running operations:

```csharp
public class AgentBackgroundService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Background processing logic
    }
}
```

**Benefits:**
- Proper application lifecycle management
- Graceful shutdown handling
- Integration with .NET hosting model
- Cancellation token support

### 4. Configuration Pattern

Hierarchical configuration using .NET configuration system:

```csharp
// appsettings.json -> appsettings.{Environment}.json -> Environment Variables
var configuration = hostContext.Configuration;
```

**Benefits:**
- Environment-specific overrides
- Type-safe configuration binding
- External configuration support
- Secret management integration

### 5. Logging Pattern

Structured logging with Serilog:

```csharp
_logger.LogInformation("Processing request: {Request}", request);
```

**Benefits:**
- Structured data for analysis
- Multiple output sinks
- Log level filtering
- Rich context information

## Technical Decisions

### 1. Semantic Kernel Choice

**Decision:** Use Microsoft Semantic Kernel as the AI orchestration framework

**Rationale:**
- Native .NET integration
- Plugin-based architecture
- Microsoft ecosystem compatibility
- Active development and support
- Function calling capabilities

**Trade-offs:**
- Microsoft ecosystem dependency
- Relatively new framework
- Limited community compared to alternatives

### 2. HTTP Client Management

**Decision:** Use `IHttpClientFactory` for HTTP client management

**Rationale:**
- Proper connection pooling
- DNS change handling
- Built-in retry policies
- Memory management
- Configuration support

**Implementation:**
```csharp
services.AddHttpClient();
var httpClient = httpClientFactory.CreateClient();
```

### 3. Error Handling Strategy

**Decision:** Implement comprehensive error handling at multiple levels

**Layers:**
1. **Plugin Level:** Individual function error handling
2. **Service Level:** Business logic error handling  
3. **Application Level:** Global exception handling

**Implementation:**
```csharp
try
{
    // Operation
    _logger.LogInformation("Operation successful");
}
catch (Exception ex)
{
    _logger.LogError(ex, "Operation failed");
    throw; // Re-throw to maintain stack trace
}
```

### 4. Configuration Management

**Decision:** Use hierarchical configuration with environment overrides

**Hierarchy:**
1. `appsettings.json` (base)
2. `appsettings.{Environment}.json` (environment-specific)
3. Environment variables
4. User secrets (development)

**Benefits:**
- Environment flexibility
- Secret security
- Override capability
- Type safety

### 5. Containerization Strategy

**Decision:** Use multi-stage Docker builds

**Stages:**
1. **Base:** Runtime image (aspnet:8.0)
2. **Build:** SDK image for compilation
3. **Publish:** Application publishing
4. **Final:** Runtime with published application

**Benefits:**
- Minimal runtime image size
- Build reproducibility
- Layer caching optimization
- Security (no build tools in runtime)

## Component Responsibilities

### Program.cs
- Application bootstrapping
- Host configuration
- Dependency registration
- Logging setup

### AgentBackgroundService
- Service lifecycle management
- Background task coordination
- Shutdown signal handling
- Exception propagation

### AgentService
- Plugin initialization and management
- Request processing coordination
- Business logic implementation
- Semantic Kernel orchestration

### API Plugins
- External API integration
- HTTP communication
- Response handling
- Error translation

## Data Flow

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────────┐
│   External      │    │  AgentService    │    │    API Plugins     │
│   Request       │───▶│                  │───▶│                     │
│                 │    │  - Route request │    │  - HTTP calls       │
│                 │    │  - Orchestrate   │    │  - Data transform   │
│                 │    │  - Handle errors │    │  - Error handling   │
└─────────────────┘    └──────────────────┘    └─────────────────────┘
                              │                           │
                              ▼                           ▼
                       ┌──────────────────┐    ┌─────────────────────┐
                       │  Semantic Kernel │    │   External APIs     │
                       │                  │    │                     │
                       │  - Function call │    │  - BICS Connect     │
                       │  - Plugin mgmt   │    │  - MyNumbers        │
                       │  - Execution     │    │  - SMS Service      │
                       └──────────────────┘    └─────────────────────┘
```

## Scalability Considerations

### Horizontal Scaling
- Stateless design enables multiple instances
- Configuration externalization
- Shared nothing architecture
- Load balancer compatibility

### Performance Optimizations
- HTTP connection pooling
- Async/await throughout
- Efficient JSON serialization
- Resource disposal patterns

### Monitoring and Observability
- Structured logging
- Performance counters
- Health checks capability
- Distributed tracing ready

## Security Architecture

### Authentication & Authorization
- API key management
- OAuth 2.0 support
- Token lifecycle management
- Secure credential storage

### Communication Security
- TLS 1.2+ enforcement
- Certificate validation
- Secure headers
- Request/response validation

### Data Protection
- No persistent data storage
- Memory cleanup
- Secure configuration
- Secret management integration

## Testing Strategy

### Unit Testing
- Plugin function testing
- Service logic testing
- Configuration validation
- Mock external dependencies

### Integration Testing
- End-to-end API flows
- Plugin integration testing
- Configuration testing
- Container testing

### Performance Testing
- Load testing API calls
- Memory usage validation
- Response time measurement
- Scalability testing

## Future Enhancements

### Planned Improvements
1. **Process Orchestration:** Complex business process support
2. **Caching Layer:** Response caching for performance
3. **Circuit Breaker:** Resilience patterns implementation
4. **Metrics Collection:** Prometheus/OpenTelemetry integration
5. **WebAPI Interface:** REST API for external integration

### Extensibility Points
1. **New Plugins:** Additional API integrations
2. **Custom Processes:** Business-specific workflows
3. **Event Processing:** Event-driven architecture
4. **AI Models:** Custom model integration
5. **Persistence:** Optional data storage

## Technology Stack

### Core Framework
- **.NET 8.0:** Modern, cross-platform runtime
- **Microsoft Semantic Kernel:** AI orchestration
- **ASP.NET Core:** Web framework (ready for future API)

### Dependencies
- **Serilog:** Structured logging
- **System.Text.Json:** JSON serialization
- **Microsoft.Extensions.*** - Configuration, DI, Hosting

### Infrastructure
- **Docker:** Containerization
- **GitHub Actions:** CI/CD pipeline
- **Dependabot:** Dependency management

### Development Tools
- **Visual Studio Code/Visual Studio:** IDE support
- **GitHub Codespaces:** Cloud development
- **Docker Desktop:** Local container development

## Best Practices Implemented

### Code Quality
- Comprehensive error handling
- Async/await best practices
- Resource disposal patterns
- Logging best practices

### Security
- Input validation
- Secure configuration
- Minimal attack surface
- Dependency scanning

### Operations
- Health check endpoints (ready)
- Graceful shutdown
- Configuration validation
- Structured logging

### Maintainability
- Clear separation of concerns
- Consistent naming conventions
- Comprehensive documentation
- Automated testing ready