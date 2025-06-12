# GitHub Codespaces Setup

This guide explains how to use GitHub Codespaces for developing and testing the Agent project in a cloud-based development environment.

## What is GitHub Codespaces?

GitHub Codespaces provides a complete development environment in the cloud, allowing you to develop without needing to set up anything locally. It includes:

- Pre-configured development environment
- VS Code in the browser or desktop
- Terminal access
- Port forwarding for testing
- Full Git integration

## Getting Started with Codespaces

### Creating a Codespace

1. **From GitHub.com**:
   - Navigate to the repository: `https://github.com/kvaes/test2`
   - Click the green "Code" button
   - Select "Codespaces" tab
   - Click "Create codespace on main"

2. **From VS Code**:
   - Install the GitHub Codespaces extension
   - Open the Command Palette (Ctrl+Shift+P / Cmd+Shift+P)
   - Type "Codespaces: Create New Codespace"
   - Select the repository and branch

### Codespace Configuration

The repository includes a `.devcontainer` configuration that automatically sets up:

- .NET 8.0 SDK
- Docker
- Required VS Code extensions
- Pre-configured settings

## Development Workflow in Codespaces

### Initial Setup

Once your Codespace is created and ready:

1. **Open the terminal** (Ctrl+` or View > Terminal)

2. **Navigate to the agent directory**:
   ```bash
   cd agent
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Build the project**:
   ```bash
   dotnet build
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```

### Running the Agent

```bash
# Start the agent application
cd agent
dotnet run

# The application will start and display logs
# Press Ctrl+C to stop
```

### Testing the Application

```bash
# Run tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal
```

### Docker Development

```bash
# Build the Docker image
docker build -t agent .

# Run the container
docker run --rm agent
```

## End-to-End Testing

### Setting up Full Environment

For complete end-to-end testing in Codespaces, you can run both the agent and any client applications:

1. **Terminal 1 - Agent**:
   ```bash
   cd agent
   dotnet run
   ```

2. **Terminal 2 - Client/Testing**:
   ```bash
   # Run client applications or API tests here
   # Example: curl commands, API tests, etc.
   ```

### Port Forwarding

Codespaces automatically forwards ports. If you need to expose additional ports:

1. Open the "Ports" tab in VS Code
2. Click "Add Port"
3. Enter the port number
4. Set visibility (Private/Public)

### Environment Variables

Set environment variables in Codespaces:

```bash
# Temporary (current session)
export ASPNETCORE_ENVIRONMENT=Development

# Persistent (add to ~/.bashrc)
echo 'export ASPNETCORE_ENVIRONMENT=Development' >> ~/.bashrc
```

## Configuration for Codespaces

### .devcontainer Configuration

The repository includes `.devcontainer/devcontainer.json`:

```json
{
  "name": "Agent Development",
  "image": "mcr.microsoft.com/devcontainers/dotnet:8.0",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {},
    "ghcr.io/devcontainers/features/github-cli:1": {}
  },
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csharp",
        "ms-dotnettools.dotnet-interactive-vscode",
        "ms-azuretools.vscode-docker",
        "ms-vscode.vscode-json",
        "eamodio.gitlens"
      ],
      "settings": {
        "dotnet.defaultSolution": "agent/Agent.csproj",
        "editor.formatOnSave": true,
        "editor.codeActionsOnSave": {
          "source.fixAll": true
        }
      }
    }
  },
  "forwardPorts": [5000, 5001],
  "postCreateCommand": "cd agent && dotnet restore",
  "remoteUser": "vscode"
}
```

### VS Code Settings

The Codespace will automatically configure:

- .NET development tools
- Docker support
- Git integration
- Code formatting and linting
- IntelliSense and debugging

## Testing Scenarios

### Unit Testing

```bash
# Run all unit tests
dotnet test

# Run specific test project
dotnet test agent.Tests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Integration Testing

```bash
# Start the agent in one terminal
cd agent
dotnet run

# In another terminal, run integration tests
curl -X GET http://localhost:5000/health
# or run your integration test suite
```

### API Testing

Use the integrated terminal to test API endpoints:

```bash
# Test Connect API plugin (example)
curl -X POST http://localhost:5000/api/connect \
  -H "Content-Type: application/json" \
  -d '{"message": "test connection"}'

# Test MyNumbers API plugin (example)
curl -X GET http://localhost:5000/api/mynumbers/status
```

## Collaborative Development

### Sharing Codespaces

1. **Share a running Codespace**:
   - Open the Command Palette
   - Type "Codespaces: Share Active Codespace"
   - Share the generated link

2. **Live Share for Real-time Collaboration**:
   - Install Live Share extension
   - Start a session
   - Invite collaborators

### Code Reviews

- Use the GitHub extension in VS Code
- View and comment on pull requests
- Make changes directly in the Codespace

## Best Practices

### Resource Management

- **Stop Codespaces** when not in use to conserve resources
- **Delete unused Codespaces** regularly
- **Monitor usage** in GitHub settings

### Development Practices

1. **Commit frequently** - Changes are saved to your Codespace
2. **Push changes** regularly to avoid data loss
3. **Use branches** for feature development
4. **Test thoroughly** before merging

### Performance Tips

- **Use smaller machine types** for basic development
- **Upgrade machine type** for intensive tasks (building, testing)
- **Use prebuilds** for faster startup times

## Troubleshooting

### Common Issues

1. **Codespace won't start**:
   - Check organization/repository permissions
   - Verify Codespaces is enabled
   - Try creating a new Codespace

2. **Slow performance**:
   - Upgrade machine type
   - Close unnecessary browser tabs
   - Check for resource-intensive processes

3. **Port forwarding issues**:
   - Manually add ports in VS Code
   - Check application binding (0.0.0.0 vs localhost)
   - Verify firewall settings

### Getting Help

- Check [GitHub Codespaces documentation](https://docs.github.com/en/codespaces)
- Use the "Report Issue" command in VS Code
- Check the repository's issue tracker
- Contact the development team

## Advanced Features

### Prebuilds

For faster Codespace creation, configure prebuilds:

1. Go to repository Settings > Codespaces
2. Set up prebuild configuration
3. Configure triggers (push, PR, schedule)

### Secrets Management

Set up secrets for API keys and sensitive data:

1. Go to Settings > Codespaces
2. Add secrets at user or repository level
3. Access in Codespace via environment variables

### Custom Dotfiles

Personalize your Codespace environment:

1. Create a dotfiles repository
2. Configure in GitHub settings
3. Automatically applied to new Codespaces

## Next Steps

Once you're comfortable with Codespaces:

1. Explore [advanced debugging techniques](debugging.md)
2. Learn about [deployment strategies](deployment.md)
3. Check out the [API plugin development guide](api-plugins.md)
4. Review [performance optimization tips](performance.md)