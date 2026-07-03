# Link Spider 🕷️

[![Build Status](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Link Spider** is a professional, high-performance, and fully asynchronous .NET 10.0 C# 14 platform for website crawling, broken link discovery, and XML sitemap generation. 

It is designed for both **developers** looking for a reusable library, and **IT/SEO professionals** who want a ready-to-use, precompiled command-line tool with zero dependencies.

---

## 📐 Dual-Purpose Architecture

This repository is strictly decoupled into two core components:

1. **`SiteMapperBash` (The Command-Line Tool):** A pre-compiled, highly efficient command-line utility. Download the native file for your platform, extract, and run immediately.
2. **`SiteMapperLib` (The Developer Library):** An open-source, thread-safe, and fully asynchronous crawling library to use inside other .NET projects.

---

## 💻 1. Standalone Command-Line Tool (`LinkSpiderConsole`)

For users who want to run site-wide crawls, check broken links, and generate sitemaps without installing any development tools or writing code.

### 🚀 Direct Download (No .NET Installation Required)

These binaries are fully **self-contained** and **single-file**. They include the entire lightweight .NET 10.0 runtime pre-packaged inside, so they run anywhere out-of-the-box.

| Operating System | Format | Processor | Direct Download Link |
| :--- | :---: | :--- | :--- |
| **Windows** | `.zip` | x64 (64-bit) | [📥 Download for Windows (x64)](https://github.com/JuanKRuiz/LinkSpider/releases/download/v2.0.0/LinkSpiderConsole-win-x64.zip) |
| **Linux** | `.tar.gz` | x64 (64-bit) | [📥 Download for Linux (x64)](https://github.com/JuanKRuiz/LinkSpider/releases/download/v2.0.0/LinkSpiderConsole-linux-x64.tar.gz) |
| **macOS (Apple Silicon)** | `.tar.gz` | ARM64 (M1/M2/M3/M4) | [📥 Download for macOS (Apple Silicon)](https://github.com/JuanKRuiz/LinkSpider/releases/download/v2.0.0/LinkSpiderConsole-osx-arm64.tar.gz) |
| **macOS (Intel)** | `.tar.gz` | x64 (64-bit) | [📥 Download for macOS (Intel x64)](https://github.com/JuanKRuiz/LinkSpider/releases/download/v2.0.0/LinkSpiderConsole-osx-x64.tar.gz) |

> [!TIP]
> **Quick Start for Linux / macOS Users:**
> After downloading, remember to grant execution permissions to the binary:
> ```bash
> tar -xzf LinkSpiderConsole-linux-x64.tar.gz
> chmod +x LinkSpiderConsole
> ./LinkSpiderConsole --url https://yoursite.com
> ```

### 🏃 Running the Executable

Run the binary directly from your terminal (**avoiding slow `dotnet run` calls**):

```bash
# On Linux / macOS:
./LinkSpiderConsole --url https://yoursite.com

# On Windows:
LinkSpiderConsole.exe --url https://yoursite.com
```

### ⚙️ Command-Line Options

The utility is fully configurable via parameters:

| Flag | Parameter | Description |
| :--- | :--- | :--- |
| `-u` | `--url=VALUE` | **Required.** The target website URL to start link exploration. |
| `-s` | `--sitemap=VALUE`| Custom filename for the generated XML sitemap (Default: `sitemap.xml`). |
| `-p` | `--plain=VALUE`  | Custom filename for the list of discovered internal links (Default: `plain.txt`). |
| `-n` | `--navfilter=VAL`| Comma-separated paths to ignore during crawling (e.g., `/tag/,/pages/`). |
| `-m` | `--smapfilter=V` | Comma-separated paths to exclude from the sitemap (but still crawl them). |
| `-c` | `--unicode`      | Use Unicode encoding for the output sitemap.xml. |
| `-o` | `--single`       | Single page scan mode (scans the landing page only; does not recurse). |
| `-h` | `--help`         | Show the help manual and parameter list. |

### 📝 Usage Examples

* **Basic Crawl and Sitemap Generation:**
  ```bash
  ./LinkSpiderConsole --url https://example.com
  ```
* **Ignore admin and login folders:**
  ```bash
  ./LinkSpiderConsole --url https://example.com --navfilter /admin/,/login/
  ```
* **Single-page crawl (checking homepage links only):**
  ```bash
  ./LinkSpiderConsole --url https://example.com --single --plain homepage_links.txt
  ```

---

## 🛠️ 2. Developer Class Library (`SiteMapperLib`)

The library is targeted at **.NET 10.0** and exploits all modern C# 14 capabilities (such as Field-Backed Properties, Primary Constructors, and Collection Expressions) to achieve maximum performance and memory efficiency.

### Quick Start Code Snippet

To crawl a site and generate a sitemap programmatically, reference the `SiteMapperLib` assembly and run:

```csharp
using System;
using System.Threading.Tasks;
using LinkSpiderLib;

// 1. Initialize the crawling orchestrator
using var spider = new LinkSpider("https://yoursite.com");

// 2. Add exploration filters to ignore specific paths (optional)
spider.URLExplorationFilter.AddRange(["/tag/", "/wp-content/"]);

// 3. Weave the web asynchronously (highly efficient, thread-safe concurrent crawler)
await spider.WeaveWebAsync(maxDegreeOfParallelism: 8);

// 4. Access discovered link lists
Console.WriteLine($"Discovered {spider.FullUrlList.Count()} internal links.");
Console.WriteLine($"Discovered {spider.ExternalUrlList.Count()} external links.");
Console.WriteLine($"Found {spider.BrokenUrlList.Count()} broken links!");

// 5. Build and save a fully compliant XML sitemap
var tarantula = new SitemapTarantula(spider.FullUrlList);
var sitemapXml = tarantula.CreateXMLDocumentSitemap();
sitemapXml.Save("sitemap.xml");
```

---

## 🔧 3. Building and Testing from Source (Developers Only)

If you are a developer and want to modify or compile the codebase yourself, follow these instructions.

### Prerequisites
* **.NET 10.0 SDK** (with C# 14 compiler)

### Compiling the Standalone Executable

To compile a native, portable, self-contained release of the console client, execute:

```bash
# Publish the project to a local directory (dist)
dotnet publish SiteMapperBash/LinkSpiderConsole.csproj -c Release -o ./dist -p:PublishSingleFile=true --self-contained true
```

### 🧪 Running Unit Tests

Both components are thoroughly validated via an automated xUnit test suite running under .NET 10.0:

```bash
# Execute unit test suites natively
dotnet test SiteMapperTests/SiteMapperTests.csproj
```

### 💻 VS Code Workspace Integration

Open the root folder in VS Code to utilize:
- **Build Task (`Ctrl+Shift+B`):** Automatic solution compilation.
- **Debugging (`F5`):** Complete coreclr debugging with breakpoint support, pre-configured to build and execute the latest source.
- **Code Hygiene:** Formats on save and auto-organizes imports to maintain high coding standards.

---

## 📜 License

Distributed under the MIT License. See [LICENSE](LICENSE) for details.
