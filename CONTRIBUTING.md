# Contributing to Excel Entity Exporter

Thank you for your interest in contributing! This document provides guidelines for contributing to this project.

## Getting Started

### Prerequisites

- [**.NET 8 SDK**](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [**Git**](https://git-scm.com/)
- A Windows machine (the project uses WinForms)

### Setup

1. Fork the repository on GitHub
2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/excel-entity-exporter-custom.git
   cd excel-entity-exporter-custom
   ```
3. Open the solution in Visual Studio 2022 or later, or build from the command line:
   ```bash
   dotnet restore
   dotnet build
   ```

## Project Structure

```
├── src/ExcelEntityExporter/         Main application
│   ├── ExcelExporter.cs             Core export logic
│   ├── InstallerForm.cs             Install/Uninstall UI
│   ├── ProgressForm.cs              Export progress UI
│   ├── Program.cs                   Entry point
│   └── VersionInfo.cs               Version string (auto-generated in CI)
├── tests/ExcelEntityExporter.Tests/ Unit tests (xUnit)
├── .github/workflows/build.yml     CI/CD pipeline
├── ExcelEntityExporter.sln          Solution file
├── README.md                        Documentation (English)
├── README.es.md                     Documentation (Spanish)
└── CONTRIBUTING.md                  This file
```

## Development Workflow

### Branching

1. Create a feature branch from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```
2. Make your changes
3. Push to your fork and open a Pull Request

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Testing Locally

Since the CI generates `VersionInfo.cs`, you may need to create a local version for development:

```bash
# Create a local VersionInfo.cs (if it doesn't exist)
echo 'namespace ExcelEntityExporter; internal static class VersionInfo { public const string Version = "dev-local"; }' > src/ExcelEntityExporter/VersionInfo.cs
```

To test the installer, build and run:
```bash
dotnet run --project src/ExcelEntityExporter
```

To test the export, run with a file path:
```bash
dotnet run --project src/ExcelEntityExporter -- "C:\path\to\your\file.xlsx"
```

## Code Guidelines

### Language

- All code comments, variable names, and documentation should be in **English**
- UI strings displayed to end users are in **Spanish** (matching the target audience)

### Code Style

- Follow standard C# conventions
- Use `PascalCase` for public members, `camelCase` for local variables
- Add XML documentation comments (`///`) for public methods
- Keep methods focused and reasonably short

### Testing

- All new features should include tests
- Tests go in `tests/ExcelEntityExporter.Tests/`
- Use xUnit with `[Fact]` for standard tests
- Create test Excel files programmatically using ClosedXML (see existing tests for examples)
- Each test should clean up after itself (the `IDisposable` pattern handles temp files)

## Submitting Changes

### Commit Messages

- Use clear, descriptive commit messages
- Start with a verb in imperative mood (e.g., "Add", "Fix", "Update")
- Keep the first line under 72 characters

Examples:
```
Add export progress cancellation support
Fix file name sanitization for special characters
Update README with installation instructions
```

### Pull Requests

1. Ensure all tests pass (`dotnet test`)
2. Update documentation if your change affects usage
3. Keep PRs focused on a single change
4. Fill in the PR description with:
   - What the change does
   - Why the change is needed
   - Any testing you performed

### CI/CD

All pull requests trigger the CI pipeline which:
1. Restores dependencies
2. Runs all unit tests
3. Builds the application
4. Produces a `.exe` artifact

PRs with failing tests will not be merged.

## Reporting Issues

- Use GitHub Issues to report bugs or request features
- Include steps to reproduce for bug reports
- Mention your Windows version and .NET SDK version if relevant

## Code of Conduct

Be respectful and constructive in all interactions. We are here to build useful software together.
