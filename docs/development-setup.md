# Development Setup

This guide will help you set up your local development environment for the Agent project.

## Prerequisites

### Required Software

- **.NET 8.0 SDK**: Download from [Microsoft .NET](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git**: Version control system
- **Docker**: For containerized development and deployment
- **Visual Studio Code** or **Visual Studio 2022**: Recommended IDEs

### Optional Tools

- **Docker Desktop**: For easy Docker management
- **GitHub CLI**: For streamlined GitHub operations
- **Postman** or **Thunder Client**: For API testing

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/kvaes/test2.git
cd test2
```

### 2. Verify .NET Installation

```bash
dotnet --version
# Should output 8.0.x or higher
```

### 3. Restore Dependencies

```bash
cd agent
dotnet restore
```

### 4. Build the Project

```bash
dotnet build
```

### 5. Run the Application

```bash
dotnet run
```

The application will start and display logging information indicating successful initialization.

## Development Workflow

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Building for Release

```bash
dotnet build --configuration Release
```

### Publishing

```bash
dotnet publish --configuration Release --output ./publish
```

## Docker Development

### Building the Container

```bash
# From the root directory
docker build -t agent .
```

### Running the Container

```bash
docker run --rm agent
```

### Development with Docker Compose

Create a `docker-compose.yml` file for local development:

```yaml
version: '3.8'
services:
  agent:
    build: .
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    volumes:
      - ./agent:/app/source
    ports:
      - "5000:80"
```

Run with:
```bash
docker-compose up --build
```

## Configuration

### Environment Variables

Set the following environment variables for development:

```bash
# .NET Configuration
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development

# Logging
export Logging__LogLevel__Default=Information
export Logging__LogLevel__Microsoft=Warning

# API Configuration (example)
export ApiSettings__Connect__BaseUrl=https://connect.bics.com
export ApiSettings__MyNumbers__BaseUrl=https://mynumbers.bics.com
```

### appsettings.Development.json

Create or modify the development configuration file:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ApiSettings": {
    "Connect": {
      "BaseUrl": "https://connect-dev.bics.com"
    },
    "MyNumbers": {
      "BaseUrl": "https://mynumbers-dev.bics.com"
    }
  }
}
```

## IDE Setup

### Visual Studio Code

1. Install the following extensions:
   - C# Dev Kit
   - Docker
   - GitLens
   - REST Client (for API testing)

2. Configure workspace settings (`.vscode/settings.json`):
   ```json
   {
     "dotnet.defaultSolution": "agent/Agent.csproj",
     "editor.formatOnSave": true,
     "editor.codeActionsOnSave": {
       "source.fixAll": true
     }
   }
   ```

3. Set up launch configuration (`.vscode/launch.json`):
   ```json
   {
     "version": "0.2.0",
     "configurations": [
       {
         "name": "Launch Agent",
         "type": "coreclr",
         "request": "launch",
         "preLaunchTask": "build",
         "program": "${workspaceFolder}/agent/bin/Debug/net8.0/Agent.dll",
         "args": [],
         "cwd": "${workspaceFolder}/agent",
         "console": "internalConsole",
         "stopAtEntry": false
       }
     ]
   }
   ```

### Visual Studio 2022

1. Open the solution file or project file directly
2. Ensure the latest .NET 8.0 workloads are installed
3. Set the startup project to the Agent project

## Debugging

### Debugging in Visual Studio Code

1. Set breakpoints in your code
2. Press F5 or use the Run and Debug panel
3. Use the Debug Console for interactive debugging

### Debugging in Visual Studio

1. Set breakpoints in your code
2. Press F5 or click Start Debugging
3. Use the Immediate Window, Locals, and Watch windows

### Debugging with Docker

Add the following to your Dockerfile for debugging:

```dockerfile
# Development stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /src
COPY . .
RUN dotnet build -c Debug
EXPOSE 5000
CMD ["dotnet", "run", "--project", "agent/Agent.csproj"]
```

## Troubleshooting

### Common Issues

1. **Build Errors**
   - Ensure .NET 8.0 SDK is installed
   - Run `dotnet restore` to restore packages
   - Check for version conflicts in project file

2. **Docker Issues**
   - Verify Docker is running
   - Check Dockerfile syntax
   - Ensure proper file permissions

3. **Configuration Issues**
   - Verify appsettings.json syntax
   - Check environment variable names
   - Ensure configuration binding is correct

### Getting Help

- Check the [troubleshooting guide](troubleshooting.md)
- Search existing GitHub issues
- Create a new issue with detailed information
- Check the project's discussions section

## Next Steps

Once your development environment is set up:

1. Read the [Architecture Guide](architecture.md)
2. Explore the [API Plugins Documentation](api-plugins.md)
3. Check out the [Contributing Guidelines](../CONTRIBUTING.md)
4. Set up [GitHub Codespaces](codespaces-setup.md) for cloud development