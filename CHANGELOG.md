# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2026-07-03] - Complete Technological Modernization (.NET 10 & C# 14)

### ✨ New Features
- **Field-Backed Properties (C# 14 `field` keyword):** Adopted the new C# 14 `field` keyword in `LinkElement.Url` (to automatically sanitize URLs during assignment) and in `LinkSpider.URLExplorationFilter` (to implement null-safe default guards), drastically reducing boilerplate code from private backing fields.

### 🔧 Refactoring and Improvements
- **Infrastructure Upgrade (.NET 10.0):** Completely migrated all projects in the repository (`LinkSpiderConsole.csproj`, `LinkSpider.csproj`, and `SiteMapperTests.csproj`) to the latest .NET 10.0 framework version, maximizing compiler and JIT optimizations.

### 📚 Documentation
- **Architecture and Guide Updates:** Rewrote `README.md` and `ARCHITECTURE.md` to reflect the state-of-the-art technological stack with .NET 10.0 and advanced C# 14 syntactic capabilities.

## [2026-07-03] - Architectural Modernization and Concurrency (.NET 9)

### 🔧 Refactoring and Improvements
- **Migration to .NET 9.0 (SDK-style):** Updated project files `LinkSpider.csproj` and `LinkSpiderConsole.csproj` to the modern SDK-style standard, removing obsolete dependencies and Portable Class Library (PCL) profiles.
- **Repository Hygiene:** Removed legacy unused files in modern .NET: `App.config`, `packages.config`, and redundant metadata in `Properties/AssemblyInfo.cs`.
- **Asynchronous Crawling Engine:** Completely rewrote `LinkSpider.cs` to support native end-to-end asynchronous operations (`async/await`), eliminating blocking synchronous calls (`.Result`) on network requests.
- **Concurrency Control:** Integrated `SemaphoreSlim` to safely throttle and regulate the maximum degree of parallelism for concurrent HTTP requests.
- **Practical Resource Management:** Implemented the `IDisposable` pattern on `LinkSpider` to manage the lifecycle of `HttpClient` safely and cleanly, and adapted its instantiation in `Program.cs` using the simplified C# `using var` declaration.
- **Network Resilience:** Added a default custom `User-Agent` header to prevent bot blocking from hosting web servers.
- **Native VS Code Integration and Purge:** Completely removed the Visual Studio solution file (`LinkSpider.sln`) and eliminated all traces of it. Adapted the `.vscode/settings.json` configuration (removing `dotnet.defaultSolution`) and updated the tasks in `.vscode/tasks.json` (build, publish) to point directly to `LinkSpiderConsole.csproj` and added a test runner for `SiteMapperTests.csproj`, ensuring a 100% native VS Code development environment without classic or modern Visual Studio IDE dependencies.

### 🐛 Bug Fixes
- **Race Condition Elimination:** Resolved thread safety issues when processing concurrent lists by shifting from `HashSet<T>` to concurrent thread-safe collections (`ConcurrentDictionary<string, LinkElement>` and `ConcurrentDictionary<string, byte>`).
- **Correct Relative URL Resolution:** Solved the pathing bug that calculated relative paths absolutely against the site root. URLs are now resolved dynamically using the exact URL of the containing page as base (`new Uri(baseUri, relativeUri)`).
- **Flexible HTML Extraction:** Resolved regex parsing fragility by compiling the pattern with case-insensitive options (`RegexOptions.IgnoreCase`), allowing anchor tags to be captured regardless of casing or quote formatting (single or double).

### 📚 Documentation
- **Development Guides Update (README.md):** Corrected installation, compilation, and unit testing commands (`dotnet build` and `dotnet test`) to target individual project files (`.csproj`) instead of the deleted solution (`.sln`) file.
