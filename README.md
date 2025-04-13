# MCP Tools Repository

This repository contains a collection of tools built using the Model Context Protocol (MCP). The primary goal is to develop various MCP tools for local usage, leveraging .NET Aspire for service orchestration and C# as the main programming language.

## About the Project

The MCP Tools project aims to provide developers with a set of utilities for working with the Model Context Protocol, making it easier to build, debug, and test MCP-enabled applications locally.

## Technology Stack

- **Programming Language:** C# (latest stable version)
- **Framework:** .NET (latest stable version)
- **Orchestration:** .NET Aspire
- **Development Environment:** Visual Studio / VS Code

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (latest stable version)
- Visual Studio 2022 or VS Code with C# extension

### Setup and Running

1. Clone the repository
2. Open the solution in Visual Studio or VS Code
3. Build and run the AppHost project to start all services

```bash
dotnet build
cd AppHost
dotnet run
```

## Project Structure

- **AppHost**: .NET Aspire orchestration project that manages all MCP tools as services
- Each tool in the repository is organized as a separate project with its own responsibility

## .NET Aspire Integration

This project leverages .NET Aspire for service orchestration. Services and resources are defined in the AppHost project, making it easy to:

- Discover and communicate between services
- Configure and manage services
- Monitor service health and logs

## Documentation and Resources

### .NET Aspire Documentation

- [Overview](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [App Host Project](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/app-host-project)
- [Service Defaults](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-defaults)

### MCP (Model Context Protocol)

- [Official C# SDK Repository](https://github.com/modelcontextprotocol/csharp-sdk)
- [Getting Started (Server)](https://github.com/modelcontextprotocol/csharp-sdk/blob/main/README.md#getting-started-server)
- [MCP Inspector](https://github.com/modelcontextprotocol/inspector)
- [SDK Samples](https://github.com/modelcontextprotocol/csharp-sdk/tree/main/samples)
