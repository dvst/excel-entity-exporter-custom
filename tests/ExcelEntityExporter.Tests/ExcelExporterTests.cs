using ClosedXML.Excel;
using Xunit;

namespace ExcelEntityExporter.Tests;

/// <summary>
/// Unit tests for ExcelExporter. Each test creates a temporary .xlsx file
/// programmatically using ClosedXML, runs the export logic, and verifies results.
/// All temp files are cleaned up after each test.
/// </summary>
public class ExcelExporterTests : IDisposable
{
    private readonly string _tempDir;

    public ExcelExporterTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ExcelExporterTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    /// <summary>
    /// Creates a minimal .xlsx file with headers and data rows.
    /// Headers: RAZON SOCIAL, FECHA INICIO COBERTURA, MONTO
    /// </summary>
    private string CreateTestWorkbook(List<(string Entity, DateTime Date, decimal Amount)> rows)
    {
        string filePath = Path.Combine(_tempDir, $"test_{Guid.NewGuid():N}.xlsx");

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Sheet1");

        // Headers
        ws.Cell(1, 1).Value = "RAZON SOCIAL";
        ws.Cell(1, 2).Value = "FECHA INICIO COBERTURA";
        ws.Cell(1, 3).Value = "MONTO";

        // Data rows
        for (int i = 0; i < rows.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = rows[i].Entity;
            ws.Cell(i + 2, 2).Value = rows[i].Date;
            ws.Cell(i + 2, 3).Value = rows[i].Amount;
        }

        workbook.SaveAs(filePath);
        return filePath;
    }

    private string CreateTestWorkbookWithHeaders(string[] headers, List<string[]> rows)
    {
        string filePath = Path.Combine(_tempDir, $"test_{Guid.NewGuid():N}.xlsx");

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Sheet1");

        for (int col = 0; col < headers.Length; col++)
            ws.Cell(1, col + 1).Value = headers[col];

        for (int row = 0; row < rows.Count; row++)
            for (int col = 0; col < rows[row].Length; col++)
                ws.Cell(row + 2, col + 1).Value = rows[row][col];

        workbook.SaveAs(filePath);
        return filePath;
    }

    // =========================================================================
    // CountCombinations tests
    // =========================================================================

    [Fact]
    public void CountCombinations_NormalFile_ReturnsCorrectCount()
    {
        var rows = new List<(string, DateTime, decimal)>
        {
            ("Empresa A", new DateTime(2025, 1, 15), 1000),
            ("Empresa A", new DateTime(2025, 2, 10), 2000),
            ("Empresa B", new DateTime(2025, 1, 20), 3000),
            ("Empresa A", new DateTime(2025, 1, 25), 1500), // duplicate combo
        };

        string filePath = CreateTestWorkbook(rows);
        int count = ExcelExporter.CountCombinations(filePath);

        Assert.Equal(3, count); // A|Jan, A|Feb, B|Jan
    }

    [Fact]
    public void CountCombinations_NoEntityColumn_ReturnsZero()
    {
        string filePath = CreateTestWorkbookWithHeaders(
            ["NOMBRE", "FECHA INICIO COBERTURA", "MONTO"],
            [["Empresa A", "2025-01-15", "1000"]]);

        int count = ExcelExporter.CountCombinations(filePath);
        Assert.Equal(0, count);
    }

    [Fact]
    public void CountCombinations_NoDateColumn_ReturnsZero()
    {
        string filePath = CreateTestWorkbookWithHeaders(
            ["RAZON SOCIAL", "FECHA PAGO", "MONTO"],
            [["Empresa A", "2025-01-15", "1000"]]);

        int count = ExcelExporter.CountCombinations(filePath);
        Assert.Equal(0, count);
    }

    [Fact]
    public void CountCombinations_EmptySheet_ReturnsZero()
    {
        string filePath = CreateTestWorkbookWithHeaders(
            ["RAZON SOCIAL", "FECHA INICIO COBERTURA", "MONTO"],
            []);

        int count = ExcelExporter.CountCombinations(filePath);
        Assert.Equal(0, count);
    }

    [Fact]
    public void CountCombinations_DuplicateRows_Deduplicates()
    {
        var rows = new List<(string, DateTime, decimal)>
        {
            ("Empresa A", new DateTime(2025, 3, 10), 1000),
            ("Empresa A", new DateTime(2025, 3, 15), 2000),
            ("Empresa A", new DateTime(2025, 3, 20), 3000),
        };

        string filePath = CreateTestWorkbook(rows);
        int count = ExcelExporter.CountCombinations(filePath);

        Assert.Equal(1, count); // all same entity+month+year
    }

    [Fact]
    public void CountCombinations_TextDateCell_SkipsRow()
    {
        string filePath = Path.Combine(_tempDir, $"test_{Guid.NewGuid():N}.xlsx");

        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.Worksheets.Add("Sheet1");
            ws.Cell(1, 1).Value = "RAZON SOCIAL";
            ws.Cell(1, 2).Value = "FECHA INICIO COBERTURA";
            ws.Cell(1, 3).Value = "MONTO";

            // Row with text (not a date) — should be skipped
            ws.Cell(2, 1).Value = "Empresa A";
            ws.Cell(2, 2).Value = "not-a-date";
            ws.Cell(2, 3).Value = "1000";

            // Row with a real DateTime — should be counted
            ws.Cell(3, 1).Value = "Empresa A";
            ws.Cell(3, 2).Value = new DateTime(2025, 6, 15);
            ws.Cell(3, 3).Value = "2000";

            workbook.SaveAs(filePath);
        }

        int count = ExcelExporter.CountCombinations(filePath);
        Assert.Equal(1, count); // only the valid DateTime row counts
    }

    [Fact]
    public void CountCombinations_EmptyEntity_SkipsRow()
    {
        string filePath = Path.Combine(_tempDir, $"test_{Guid.NewGuid():N}.xlsx");

        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.Worksheets.Add("Sheet1");
            ws.Cell(1, 1).Value = "RAZON SOCIAL";
            ws.Cell(1, 2).Value = "FECHA INICIO COBERTURA";
            ws.Cell(1, 3).Value = "MONTO";

            // Row with empty entity — should be skipped
            ws.Cell(2, 1).Value = "";
            ws.Cell(2, 2).Value = new DateTime(2025, 1, 15);
            ws.Cell(2, 3).Value = "1000";

            // Row with valid entity — should be counted
            ws.Cell(3, 1).Value = "Empresa A";
            ws.Cell(3, 2).Value = new DateTime(2025, 1, 20);
            ws.Cell(3, 3).Value = "2000";

            workbook.SaveAs(filePath);
        }

        int count = ExcelExporter.CountCombinations(filePath);
        Assert.Equal(1, count); // only the row with entity counts
    }

    // =========================================================================
    // Export tests
    // =========================================================================

    [Fact]
    public void Export_FileNotFound_ReturnsError()
    {
        var result = ExcelExporter.Export("/nonexistent/path/file.xlsx");

        Assert.False(result.Success);
        Assert.Contains("File not found", result.ErrorMessage);
    }

    [Fact]
    public void Export_ValidFile_CreatesCorrectFiles()
    {
        var rows = new List<(string, DateTime, decimal)>
        {
            ("Empresa A", new DateTime(2025, 1, 15), 1000),
            ("Empresa A", new DateTime(2025, 1, 20), 2000),
            ("Empresa B", new DateTime(2025, 3, 10), 3000),
        };

        string filePath = CreateTestWorkbook(rows);
        var result = ExcelExporter.Export(filePath);

        Assert.True(result.Success);
        Assert.Equal(2, result.FilesCreated);
        Assert.True(Directory.Exists(result.OutputFolder));

        var files = Directory.GetFiles(result.OutputFolder, "*.xlsx");
        Assert.Equal(2, files.Length);

        // Verify file names contain expected content
        Assert.Contains(files, f => f.Contains("ENERO") && f.Contains("2025") && f.Contains("Empresa A"));
        Assert.Contains(files, f => f.Contains("MARZO") && f.Contains("2025") && f.Contains("Empresa B"));
    }

    [Fact]
    public void Export_MissingEntityColumn_ReturnsError()
    {
        string filePath = CreateTestWorkbookWithHeaders(
            ["NOMBRE", "FECHA INICIO COBERTURA", "MONTO"],
            [["Empresa A", "2025-01-15", "1000"]]);

        var result = ExcelExporter.Export(filePath);

        Assert.False(result.Success);
        Assert.Contains("RAZON SOCIAL", result.ErrorMessage);
    }

    [Fact]
    public void Export_MissingDateColumn_ReturnsError()
    {
        string filePath = CreateTestWorkbookWithHeaders(
            ["RAZON SOCIAL", "FECHA PAGO", "MONTO"],
            [["Empresa A", "2025-01-15", "1000"]]);

        var result = ExcelExporter.Export(filePath);

        Assert.False(result.Success);
        Assert.Contains("FECHA INICIO COBERTURA", result.ErrorMessage);
    }

    [Fact]
    public void Export_EntityWithSpecialChars_FileNameCleaned()
    {
        var rows = new List<(string, DateTime, decimal)>
        {
            ("ACME/Corp: Inc.", new DateTime(2025, 6, 15), 5000),
        };

        string filePath = CreateTestWorkbook(rows);
        var result = ExcelExporter.Export(filePath);

        Assert.True(result.Success);
        Assert.Equal(1, result.FilesCreated);

        var files = Directory.GetFiles(result.OutputFolder, "*.xlsx");
        string fileName = Path.GetFileName(files[0]);

        // Should not contain / or :
        Assert.DoesNotContain("/", fileName);
        Assert.DoesNotContain(":", fileName);
        Assert.Contains("ACMECorp", fileName);
    }

    [Fact]
    public void Export_EntityNameTooLong_Truncated()
    {
        string longName = new string('A', 150); // 150 chars
        var rows = new List<(string, DateTime, decimal)>
        {
            (longName, new DateTime(2025, 6, 15), 5000),
        };

        string filePath = CreateTestWorkbook(rows);
        var result = ExcelExporter.Export(filePath);

        Assert.True(result.Success);

        var files = Directory.GetFiles(result.OutputFolder, "*.xlsx");
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(files[0]);

        // File name should be truncated (100 chars max + "IND JUNIO 2025 " prefix)
        Assert.True(fileNameWithoutExt.Length < longName.Length + 30);
    }

    [Fact]
    public void Export_CancelledToken_ReturnsCancelled()
    {
        var rows = new List<(string, DateTime, decimal)>
        {
            ("Empresa A", new DateTime(2025, 1, 15), 1000),
        };

        string filePath = CreateTestWorkbook(rows);

        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Pre-cancel

        var result = ExcelExporter.Export(filePath, cancellationToken: cts.Token);

        Assert.False(result.Success);
        Assert.Contains("cancelled", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Export_EmptyWorksheet_SuccessNoFiles()
    {
        string filePath = CreateTestWorkbookWithHeaders(
            ["RAZON SOCIAL", "FECHA INICIO COBERTURA", "MONTO"],
            []);

        var result = ExcelExporter.Export(filePath);

        Assert.True(result.Success);
        Assert.Equal(0, result.FilesCreated);
    }

    [Fact]
    public void Export_ProgressReporting_ReportsPercentage()
    {
        var rows = new List<(string, DateTime, decimal)>
        {
            ("Empresa A", new DateTime(2025, 1, 15), 1000),
            ("Empresa B", new DateTime(2025, 2, 10), 2000),
            ("Empresa C", new DateTime(2025, 3, 20), 3000),
        };

        string filePath = CreateTestWorkbook(rows);

        var reportedValues = new List<int>();
        var progress = new Progress<int>(v => reportedValues.Add(v));

        var result = ExcelExporter.Export(filePath, progress);

        Assert.True(result.Success);
        Assert.Equal(3, result.FilesCreated);
        Assert.NotEmpty(reportedValues);
        Assert.All(reportedValues, v => Assert.InRange(v, 0, 100));
    }
}
