using ClosedXML.Excel;

namespace ExcelEntityExporter;

/// <summary>
/// Core export logic. Reads an Excel file and splits it into separate .xlsx files,
/// one per unique Entity + Month + Year combination.
/// Mirrors the VBA macro from "ARCHIVO IND FINAL.bas".
/// </summary>
public class ExcelExporter
{
    private const string EntityColumnPatterns = "RAZON SOCIAL|ENTIDAD";
    private const string DateColumnPattern = "FECHA INICIO COBERTURA";
    private const string OutputFolderName = "Archivos_por_Entidad";
    private const int MaxFileNameLength = 100;

    private static readonly string[] InvalidFileNameChars =
        ["\\", "/", ":", "*", "?", "\"", "<", ">", "|"];

    private static readonly Dictionary<int, string> MonthNames = new()
    {
        { 1, "ENERO" }, { 2, "FEBRERO" }, { 3, "MARZO" },
        { 4, "ABRIL" }, { 5, "MAYO" }, { 6, "JUNIO" },
        { 7, "JULIO" }, { 8, "AGOSTO" }, { 9, "SEPTIEMBRE" },
        { 10, "OCTUBRE" }, { 11, "NOVIEMBRE" }, { 12, "DICIEMBRE" }
    };

    public class ExportResult
    {
        public bool Success { get; set; }
        public string OutputFolder { get; set; } = string.Empty;
        public int FilesCreated { get; set; }
        public List<string> Warnings { get; set; } = [];
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Counts how many unique Entity+Month+Year combinations exist in the file.
    /// Used to set up the progress bar before the actual export.
    /// </summary>
    public static int CountCombinations(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        var ws = workbook.Worksheet(1);
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

        int entityCol = FindColumn(ws, lastCol, EntityColumnPatterns);
        int dateCol = FindColumn(ws, lastCol, DateColumnPattern);

        if (entityCol == 0 || dateCol == 0)
            return 0;

        var combinations = new HashSet<string>();

        for (int row = 2; row <= lastRow; row++)
        {
            string entity = ws.Cell(row, entityCol).GetString().Trim();
            var dateCell = ws.Cell(row, dateCol);

            if (!string.IsNullOrEmpty(entity) && dateCell.DataType == XLDataType.DateTime)
            {
                var date = dateCell.GetDateTime();
                string key = $"{entity}|{date.Month}|{date.Year}";
                combinations.Add(key);
            }
        }

        return combinations.Count;
    }

    /// <summary>
    /// Performs the full export: reads the source file, groups rows by
    /// Entity+Month+Year, and writes each group to its own .xlsx file.
    /// </summary>
    public static ExportResult Export(
        string filePath,
        IProgress<int>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new ExportResult();

        try
        {
            if (!File.Exists(filePath))
            {
                result.ErrorMessage = $"File not found: {filePath}";
                return result;
            }

            string outputFolder = Path.Combine(
                Path.GetDirectoryName(filePath)!,
                OutputFolderName);
            Directory.CreateDirectory(outputFolder);
            result.OutputFolder = outputFolder;

            using var sourceWorkbook = new XLWorkbook(filePath);
            var ws = sourceWorkbook.Worksheet(1);
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
            var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

            int entityCol = FindColumn(ws, lastCol, EntityColumnPatterns);
            int dateCol = FindColumn(ws, lastCol, DateColumnPattern);

            if (entityCol == 0)
            {
                result.ErrorMessage = "Column 'RAZON SOCIAL' or 'ENTIDAD' not found in the worksheet headers.";
                return result;
            }

            if (dateCol == 0)
            {
                result.ErrorMessage = "Column 'FECHA INICIO COBERTURA' not found in the worksheet headers.";
                return result;
            }

            // First pass: collect all unique Entity+Month+Year combinations
            var combinations = new Dictionary<string, (string Entity, int Month, string MonthName, int Year)>();

            for (int row = 2; row <= lastRow; row++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string entity = ws.Cell(row, entityCol).GetString().Trim();
                var dateCell = ws.Cell(row, dateCol);

                if (!string.IsNullOrEmpty(entity) && dateCell.DataType == XLDataType.DateTime)
                {
                    var date = dateCell.GetDateTime();
                    string key = $"{entity}|{date.Month}|{date.Year}";

                    if (!combinations.ContainsKey(key))
                    {
                        string monthName = MonthNames[date.Month];
                        combinations[key] = (entity, date.Month, monthName, date.Year);
                    }
                }
            }

            int totalCombinations = combinations.Count;
            int processed = 0;

            // Second pass: create one file per combination
            foreach (var combo in combinations.Values)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string fileName = $"IND {combo.MonthName} {combo.Year} {CleanFileName(combo.Entity)}.xlsx";
                string outputPath = Path.Combine(outputFolder, fileName);

                using var newWorkbook = new XLWorkbook();
                var newSheet = newWorkbook.Worksheets.Add("Sheet1");

                // Copy headers
                for (int col = 1; col <= lastCol; col++)
                {
                    newSheet.Cell(1, col).Value = ws.Cell(1, col).Value;
                }

                int destRow = 2;

                // Copy matching rows
                for (int row = 2; row <= lastRow; row++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string entity = ws.Cell(row, entityCol).GetString().Trim();
                    var dateCell = ws.Cell(row, dateCol);

                    if (entity == combo.Entity &&
                        dateCell.DataType == XLDataType.DateTime)
                    {
                        var date = dateCell.GetDateTime();

                        if (date.Month == combo.Month && date.Year == combo.Year)
                        {
                            for (int col = 1; col <= lastCol; col++)
                            {
                                newSheet.Cell(destRow, col).Value = ws.Cell(row, col).Value;
                            }
                            destRow++;
                        }
                    }
                }

                // Auto-fit columns
                newSheet.Columns().AdjustToContents();

                newWorkbook.SaveAs(outputPath);
                result.FilesCreated++;

                processed++;
                progress?.Report((int)((double)processed / totalCombinations * 100));
            }

            result.Success = true;
        }
        catch (OperationCanceledException)
        {
            result.ErrorMessage = "Export was cancelled by the user.";
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"Unexpected error: {ex.Message}";
        }

        return result;
    }

    /// <summary>
    /// Scans header row (row 1) to find a column whose name contains any of the
    /// pipe-separated patterns. Returns the 1-based column index, or 0 if not found.
    /// </summary>
    private static int FindColumn(IXLWorksheet ws, int lastCol, string patterns)
    {
        string[] searchTerms = patterns.Split('|');

        for (int col = 1; col <= lastCol; col++)
        {
            string header = ws.Cell(1, col).GetString().Trim().ToUpperInvariant();

            foreach (string term in searchTerms)
            {
                if (header.Contains(term.ToUpperInvariant()))
                    return col;
            }
        }

        return 0;
    }

    /// <summary>
    /// Removes invalid file name characters and truncates to MaxFileNameLength.
    /// </summary>
    private static string CleanFileName(string name)
    {
        foreach (string c in InvalidFileNameChars)
        {
            name = name.Replace(c, "");
        }

        if (name.Length > MaxFileNameLength)
            name = name[..MaxFileNameLength];

        return name.Trim();
    }
}
