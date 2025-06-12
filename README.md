# Agent Project

A C# Semantic Kernel-based agent project designed for processing requests through various BICS APIs including Connect, MyNumbers, and SMS services.

## Features

- **Semantic Kernel Integration**: Built on Microsoft's Semantic Kernel framework for AI orchestration
- **Multiple API Support**: Integrates with various BICS APIs including:
  - Connect API
  - MyNumbers API (with Address Management, CDR, Disconnection, Emergency Services, and Number Porting)
  - SMS API
- **Containerized Deployment**: Docker support for easy deployment
- **Comprehensive Logging**: Structured logging with Serilog
- **CI/CD Pipeline**: Automated build, test, and container creation
- **Security-First**: Built with security best practices and vulnerability scanning

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- Docker (for containerized deployment)
- Visual Studio Code or Visual Studio (recommended)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/kvaes/test2.git
   cd test2
   ```

2. **Build and run the agent**
   ```bash
   cd agent
   dotnet restore
   dotnet build
   dotnet run
   ```

3. **Run with Docker**
   ```bash
   docker build -t agent .
   docker run agent
   ```

### GitHub Codespaces

This project is configured for GitHub Codespaces development. Click the "Code" button and select "Create codespace on main" to get started with a fully configured development environment.

For detailed setup instructions, see [Development Setup](docs/development-setup.md).

## Architecture

The project follows a clean architecture pattern with the following structure:

```
agent/
├── Functions/          # Semantic Kernel function definitions
├── Processes/         # Defined business processes
├── Steps/             # Reusable step definitions
├── Program.cs         # Application entry point
├── AgentService.cs    # Core agent service implementation
└── appsettings.json   # Configuration settings
```

## Configuration

The agent uses a configuration-driven approach for API endpoints and settings. Key configuration areas include:

- **API Settings**: Base URLs for various BICS APIs
- **Logging**: Structured logging configuration
- **Security**: Authentication and authorization settings

See [Configuration Guide](docs/configuration.md) for detailed information.

## API Plugins

The agent includes plugins for the following APIs:

- **Connect API**: Connection management and communication services
- **MyNumbers API**: Number management and related services
- **SMS API**: SMS messaging services

Each plugin exposes all available endpoints as Semantic Kernel functions. See [API Documentation](docs/api-plugins.md) for detailed information.

## Development

### Project Structure

- `.github/`: GitHub Actions workflows, dependabot, and security policies
- `agent/`: Main C# Semantic Kernel application
- `docs/`: Project documentation
- `Dockerfile`: Container configuration

### Development Workflow

1. Create a feature branch
2. Make your changes
3. Run tests: `dotnet test`
4. Build: `dotnet build`
5. Create a pull request

### Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and development process.

## Security

Security is a top priority. Please see our [Security Policy](.github/SECURITY.md) for information on reporting vulnerabilities and security best practices.

## CI/CD

The project uses GitHub Actions for continuous integration and deployment:

- **Build and Test**: Automated building and testing on pull requests
- **Container Build**: Automatic container creation and publishing
- **Dependency Updates**: Automated dependency updates via Dependabot

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For support and questions:

- Create an issue in the GitHub repository
- Check the [documentation](docs/)
- Review existing issues and discussions

## Links

- [Development Setup](docs/development-setup.md)
- [GitHub Codespaces Guide](docs/codespaces-setup.md)
- [API Documentation](docs/api-plugins.md)
- [Configuration Guide](docs/configuration.md)