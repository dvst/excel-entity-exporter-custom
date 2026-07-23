namespace ExcelEntityExporter;

static class Program
{
    /// <summary>
    /// Entry point. The application has two modes:
    ///   1. No arguments  → Show the Install/Uninstall UI (installer mode)
    ///   2. With arguments → Run the Excel export (context menu mode)
    ///
    /// When registered in the Windows context menu, the .exe is invoked as:
    ///   "C:\...\installer-entity-exporter.exe" "C:\path\to\file.xlsx"
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
        {
            // --- Export mode ---
            // Launched from the right-click context menu with a file path
            string filePath = args[0];

            if (!File.Exists(filePath))
            {
                MessageBox.Show(
                    $"File not found:\n{filePath}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (!IsExcelFile(filePath))
            {
                MessageBox.Show(
                    "The selected file is not a supported Excel file (.xlsx, .xls, .xlsm).",
                    "Invalid file",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Application.Run(new ProgressForm(filePath));
        }
        else
        {
            // --- Installer mode ---
            // Double-clicked without arguments → show install/uninstall UI
            Application.Run(new InstallerForm());
        }
    }

    private static bool IsExcelFile(string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext is ".xlsx" or ".xls" or ".xlsm";
    }
}
