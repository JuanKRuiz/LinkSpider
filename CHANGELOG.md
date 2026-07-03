# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2026-07-03] - Modernización Tecnológica Completa (.NET 10 & C# 14)

### ✨ New Features
- **Propiedades con Respaldo de Campo (C# 14 `field` keyword):** Adoptado el nuevo keyword `field` de C# 14 en `LinkElement.Url` (para realizar sanitizaciones de URLs automáticamente al asignarlas) y en `LinkSpider.URLExplorationFilter` (para implementar salvaguardas contra valores nulos), reduciendo drásticamente el código redundante (*boilerplate*) de backing fields privados.

### 🔧 Refactoring and Improvements
- **Actualización de Infraestructura (.NET 10.0):** Migrados por completo todos los proyectos del repositorio (`LinkSpiderConsole.csproj`, `LinkSpider.csproj`, y `SiteMapperTests.csproj`) a la versión más reciente del framework de largo plazo, **.NET 10.0**, maximizando las optimizaciones del compilador y JIT.

### 📚 Documentation
- **Actualización de Documentos de Arquitectura y Guías:** Reescritos `README.md` y `ARCHITECTURE.md` para reflejar el estado del arte tecnológico con .NET 10.0 y las capacidades sintácticas avanzadas de C# 14.

## [2026-07-03] - Modernización Arquitectónica y Concurrencia (.NET 9)

### 🔧 Refactoring and Improvements
- **Migración a .NET 9.0 (SDK-style):** Actualizados los archivos de proyecto `LinkSpider.csproj` y `LinkSpiderConsole.csproj` al estándar moderno de SDK-style, removiendo dependencias obsoletas y perfiles de Portable Class Library (PCL).
- **Higiene del Repositorio:** Removidos archivos heredados sin uso en .NET moderno: `App.config`, `packages.config`, y metadatos redundantes en `Properties/AssemblyInfo.cs`.
- **Motor de Rastreo Asincrónico:** Reescrito por completo `LinkSpider.cs` para soportar asincronismo de extremo a extremo (`async/await`) nativo, eliminando bloqueos sincrónicos (`.Result`) en peticiones de red.
- **Control de Concurrencia:** Incorporado `SemaphoreSlim` para regular de forma segura el grado de paralelismo máximo de las peticiones HTTP concurrentes.
- **Gestión de Recursos Práctica:** Implementado el patrón `IDisposable` sobre `LinkSpider` para liberar el ciclo de vida del `HttpClient` y adaptada su instanciación en `Program.cs` mediante la declaración simplificada `using var` de C#.
- **Robustez de Red:** Añadido cabezal de `User-Agent` personalizado por defecto para evitar bloqueos por políticas anti-bot de servidores web de hosting.
- **Integración y Purga para VS Code Nativo:** Removido por completo el archivo de solución de Visual Studio (`LinkSpider.sln`) y eliminados todos sus rastros en el proyecto. Adaptada la configuración de `.vscode/settings.json` (removiendo `dotnet.defaultSolution`) y adaptados los runners en `.vscode/tasks.json` (build, publish) para apuntar directamente a `LinkSpiderConsole.csproj` y añadir un runner de pruebas para `SiteMapperTests.csproj`, garantizando un entorno de desarrollo 100% nativo de VS Code sin dependencias de IDEs clásicas o modernas.

### 🐛 Bug Fixes
- **Eliminación de Condiciones de Carrera (Race Conditions):** Corregida la inseguridad de hilos al procesar listas concurrentes mediante la transición de `HashSet<T>` a colecciones thread-safe concurrentes (`ConcurrentDictionary<string, LinkElement>` y `ConcurrentDictionary<string, byte>`).
- **Resolución Correcta de URLs Relativas:** Solucionado el bug de pathing que calculaba rutas relativas de manera absoluta sobre la raíz del sitio. Ahora las URLs se resuelven dinámicamente usando el URL exacto de la página que contiene el enlace como base (`new Uri(baseUri, relativeUri)`).
- **Extracción de HTML Flexible:** Resuelta la fragilidad de extracción regex aplicando compilación y opción de insensibilidad a mayúsculas (`RegexOptions.IgnoreCase`), permitiendo capturar etiquetas de enlace en cualquier formato o estructura de comillas (simples o dobles).

### 📚 Documentation
- **Actualización de Guías de Desarrollo (README.md):** Corregidos los comandos de instalación, compilación y ejecución de pruebas unitarias (`dotnet build` y `dotnet test`) para que utilicen los archivos de proyecto `.csproj` individuales en lugar del archivo de solución `.sln` eliminado.
