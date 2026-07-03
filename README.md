# Link Spider 🕷️

[![Build Status](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Link Spider** is a professional, high-performance, and fully asynchronous .NET 10.0 C# 14 platform consisting of a **reusable open-source Class Library** (`SiteMapperLib`) and a **standalone command-line CLI executable** (`SiteMapperBash`) for website crawling, broken link discovery, and XML sitemap generation.

---

## 📐 Dual-Purpose Architecture

This repository is strictly decoupled into two core components:

1. **`SiteMapperLib` (The Library):** An open-source, thread-safe, and fully asynchronous crawling engine. You can reference this library in any .NET application to crawl websites programmatically.
2. **`SiteMapperBash` (The Command-Line Tool):** A pre-compiled, highly efficient CLI tool that consumes `SiteMapperLib` to let developers and administrators run site-wide crawls and generate XML sitemaps natively from the command line.

---

## 🛠️ 1. Developer Class Library (`SiteMapperLib`)

The library is targeted at **.NET 10.0** and exploits all modern C# 14 capabilities (such as Field-Backed Properties with the `field` keyword, Primary Constructors, and Collection Expressions) to achieve maximum performance and memory efficiency.

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

## 💻 2. Standalone Command-Line Tool (`LinkSpiderConsole`)

For users who just want to use the pre-built command-line crawling tool without writing code, `LinkSpiderConsole` is packaged as a ready-to-run binary. You can build it as a native, single-file executable or download pre-compiled versions from the [GitHub Releases](https://github.com/JuanKRuiz/LinkSpider/releases).

### Compiling the Standalone Executable

To compile a native, portable release of the console client, execute:

```bash
# 1. Publish the project to a local directory (dist)
dotnet publish SiteMapperBash/LinkSpiderConsole.csproj -c Release -o ./dist
```

### Running the Executable Directly

Once compiled or downloaded, run the binary directly from your terminal (**avoiding slow `dotnet run` calls**):

```bash
# On Linux / macOS:
./dist/LinkSpiderConsole --url https://yoursite.com

# On Windows:
dist\LinkSpiderConsole.exe --url https://yoursite.com
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

#### Examples of Executable Commands:

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

## 🧪 Testing and Integration

Both components are thoroughly validated via an automated xUnit test suite running under .NET 10.0:

```bash
# Execute unit test suites natively
dotnet test SiteMapperTests/SiteMapperTests.csproj
```

---

## ⚙️ VS Code Workspace Integration

Open the root folder in VS Code to utilize:
- **Build Task (`Ctrl+Shift+B`):** Automatic solution compilation.
- **Debugging (`F5`):** Complete coreclr debugging with breakpoint support, pre-configured to build and execute the latest source.
- **Code Hygiene:** Formats on save and auto-organizes imports to maintain high coding standards.

---

## 📜 License

Distributed under the MIT License. See `LICENSE` for details.
