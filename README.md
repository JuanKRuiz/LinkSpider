# Link Spider 🕷️

[![Build Status](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Link Spider** is a high-performance, fully asynchronous, cross-platform library and command-line utility for crawling websites, discovering broken or external links, and generating standard, compliant `sitemap.xml` files. 

Completely modernized from legacy .NET Framework to **.NET 9.0** and **C# 13**, it features thread-safe concurrent architecture, robust relative URI resolution, non-blocking async network operations, and optimized regex-based link extraction.

---

## ✨ Features

- **High-Performance Concurrent Engine:** Leverages `ConcurrentDictionary` and `SemaphoreSlim` to achieve thread-safe, throttle-controlled parallel crawling.
- **Asynchronous & Non-Blocking:** Full end-to-end `async/await` execution pipeline to prevent ThreadPool starvation and maximize throughput.
- **Perfect Relative Pathing Resolution:** Dynamically resolves relative paths using the actual page of origin URL as base (`new Uri(baseUri, relativeUri)`), avoiding broken pathing traps on nested routes.
- **Robust Case-Insensitive Matching:** Pre-compiled, highly optimized Regex matching for anchor tags (`<a href="...">`), supporting any casing, single or double quotes, and multi-attribute links.
- **Sitemap Filtering:** Full support to exclude specific URL patterns from being crawled or listed in the generated XML sitemap.
- **Cross-Platform:** Built natively on .NET 9.0, running flawlessly on Linux, macOS, and Windows.
- **Developer-Friendly VS Code Integration:** Complete with pre-configured `.vscode/` settings, compilation tasks, and debugger profiles.

---

## 📂 Project Structure

```
LinkSpider/
├── SiteMapperLib/         # Core engine (.NET 9 Class Library)
│   ├── LinkSpider.cs      # Asynchronous crawler orchestrator
│   └── SitemapTarantula.cs # Compliant XML sitemap builder
├── SiteMapperBash/        # Command-Line Interface (.NET 9 Console App)
│   └── Program.cs         # CLI execution shell & Option Parser
├── SiteMapperTests/       # Unit Testing Suite (xUnit)
│   └── SitemapTarantulaTests.cs
└── .vscode/               # VS Code workspace integration
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Installation
Clone the repository and compile the solution:
```bash
git clone https://github.com/JuanKRuiz/LinkSpider.git
cd LinkSpider
dotnet build SiteMapperLib/LinkSpider.sln
```

---

## 💻 CLI Usage

The console client (`LinkSpiderConsole`) allows rapid, configurable site scans.

### Quick Scan
Generates a standard `sitemap.xml` and a flat list `plain.txt` of all discovered internal links:
```bash
dotnet run --project SiteMapperBash/LinkSpiderConsole.csproj -- --u https://yoursite.com
```

### Advanced Usage

#### Customizing Output Files (`--s`, `--p`)
```bash
dotnet run --project SiteMapperBash/LinkSpiderConsole.csproj -- --u https://yoursite.com --s MySitemap.xml --p AllLinks.txt
```

#### Navigation Filtering (`--n`)
Prevents exploring links matching specific patterns (e.g. `/tag/` or `/pages/` folders):
```bash
dotnet run --project SiteMapperBash/LinkSpiderConsole.csproj -- --u https://yoursite.com --n /tag/,/pages/
```

#### Sitemap Exclusions (`--m`)
Crawls the links but excludes them from appearing in the generated `sitemap.xml`:
```bash
dotnet run --project SiteMapperBash/LinkSpiderConsole.csproj -- --u https://yoursite.com --m /tag/,/pages/
```

#### Single Page Mode (`--o`)
Scans and analyzes only the single provided page, without deep-crawling outward links:
```bash
dotnet run --project SiteMapperBash/LinkSpiderConsole.csproj -- --u https://yoursite.com/somepage --o
```

---

## 🧪 Running Tests

The solution includes a thorough suite of xUnit tests verifying the parser, collections, and filtering:
```bash
dotnet test SiteMapperLib/LinkSpider.sln
```

---

## ⚙️ VS Code Workspace Integration

Open the root folder in VS Code to utilize:
- **Build Task (`Ctrl+Shift+B`):** Automatic solution compilation.
- **Debugging (`F5`):** Complete coreclr debugging with breakpoint support, pre-configured to build the latest source.
- **Code Hygiene:** Formats on save and auto-organizes imports to maintain high coding standards.

---

## 📜 License

Distributed under the MIT License. See `LICENSE` for details.
