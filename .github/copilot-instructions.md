# GitHub Copilot Instructions for the MCP Tools Repository

## Context and Purpose

This repository contains tools built using the Model Context Protocol (MCP). The primary goal is to develop various MCP tools for local usage. The project utilizes .NET Aspire for service orchestration and C# as the main programming language.

## Technology Stack

-   **Programming Language:** C# (latest stable version)
-   **Framework:** .NET (latest stable version)
-   **Orchestration:** .NET Aspire
-   **Development Environment:** Visual Studio / VS Code

## Coding Style and Best Practices (C#)

-   Follow the official [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
-   Use meaningful names for variables, methods, and classes.
-   Prefer `async/await` for I/O-bound operations.
-   Utilize LINQ for data manipulation where appropriate, but prioritize readability.
-   Write unit tests for core logic using a standard testing framework (e.g., xUnit, NUnit).
-   Add XML documentation comments (`///`) to public APIs.
-   Keep methods short and focused on a single responsibility (SRP).
-   Use dependency injection (DI) for managing dependencies, leveraging .NET's built-in DI container.

## .NET Aspire Guidance

-   Define services and resources in the `AppHost` project.
-   Use the `AddProject` and `AddExecutable` methods in `Program.cs` of the `AppHost` project to include different MCP tool projects or executables.
-   Leverage Aspire's service discovery and configuration management features.
-   Consult the official .NET Aspire documentation for specific patterns and features.

## Important Documentation Links

-   **.NET Aspire Documentation:**
    -   [Overview](https://learn.microsoft.com/en-us/dotnet/aspire/)
    -   [What's New in .NET Aspire for .NET 9](https://learn.microsoft.com/en-us/dotnet/aspire/whats-new/dotnet-aspire-9?tabs=unix)
    -   [App Host Project](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/app-host-project)
    -   [Service Defaults](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/service-defaults)
-   **C# Documentation:**
    -   [C# Guide](https://learn.microsoft.com/en-us/dotnet/csharp/)
    -   [Asynchronous programming with async and await](https://learn.microsoft.com/en-us/dotnet/csharp/async)
-   **MCP (Model Context Protocol):**
    -   [Official C# SDK Repository](https://github.com/modelcontextprotocol/csharp-sdk)
    -   [Getting Started (Server)](https://github.com/modelcontextprotocol/csharp-sdk/blob/main/README.md#getting-started-server)
    -   [SDK Samples](https://github.com/modelcontextprotocol/csharp-sdk/tree/main/samples)
    -   [Announcement Blog Post](https://devblogs.microsoft.com/blog/microsoft-partners-with-anthropic-to-create-official-c-sdk-for-model-context-protocol)

## General Instructions

-   When generating code, adhere to the specified technology stack and best practices.
-   Prioritize clarity, maintainability, and performance.
-   If unsure about a specific implementation detail, refer to the provided documentation links or ask for clarification.
-   Ensure generated code integrates well with the .NET Aspire orchestration model.
-   Keep dependencies updated to the latest stable versions compatible with the project.
