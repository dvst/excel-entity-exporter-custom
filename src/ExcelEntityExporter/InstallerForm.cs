using Microsoft.Win32;

namespace ExcelEntityExporter;

/// <summary>
/// Installer form shown when the .exe is launched without arguments.
/// Provides Install and Uninstall buttons. Install copies the executable to
/// Program Files and adds a right-click context menu entry for .xlsx files.
/// Uninstall removes both.
/// </summary>
public partial class InstallerForm : Form
{
    private const string AppName = "ExcelEntityExporter";
    private const string AppDescription = "Exportar por Entidad y Fecha";
    private const string InstallSubDir = "ExcelEntityExporter";

    // Registry paths for the right-click context menu on .xlsx files
    private const string ContextMenuKey = @"Excel.Sheet.12\shell\ExportarPorEntidad";
    private const string ContextMenuCommandKey = @"Excel.Sheet.12\shell\ExportarPorEntidad\command";

    // Registry path for Add/Remove Programs (so it appears in Windows Settings > Apps)
    private const string UninstallKeyPath =
        @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ExcelEntityExporter";

    public InstallerForm()
    {
        InitializeComponent();
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        bool installed = IsInstalled();

        if (installed)
        {
            lblStatus.Text = $"Status: Installed at {GetInstallPath()}";
            btnInstall.Enabled = false;
            btnInstall.BackColor = Color.LightGray;
            btnUninstall.Enabled = true;
        }
        else
        {
            lblStatus.Text = "Status: Not installed";
            btnInstall.Enabled = true;
            btnInstall.BackColor = Color.FromArgb(0, 120, 215);
            btnUninstall.Enabled = false;
        }
    }

    private static string GetInstallPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            InstallSubDir);
    }

    private static bool IsInstalled()
    {
        string installExe = Path.Combine(GetInstallPath(), "installer-entity-exporter.exe");
        return File.Exists(installExe);
    }

    private void BtnInstall_Click(object? sender, EventArgs e)
    {
        try
        {
            string installDir = GetInstallPath();
            Directory.CreateDirectory(installDir);

            string destExe = Path.Combine(installDir, "installer-entity-exporter.exe");
            string currentExe = Environment.ProcessPath!;

            // Copy the running executable to Program Files
            File.Copy(currentExe, destExe, overwrite: true);

            // Add right-click context menu entry
            using (var shellKey = Registry.ClassesRoot.CreateSubKey(ContextMenuKey))
            {
                shellKey.SetValue("", AppDescription);
                shellKey.SetValue("Icon", $"\"{destExe}\"");
            }

            using (var cmdKey = Registry.ClassesRoot.CreateSubKey(ContextMenuCommandKey))
            {
                cmdKey.SetValue("", $"\"{destExe}\" \"%1\"");
            }

            // Add uninstall entry so it shows up in Windows Settings > Apps
            using (var uninstallKey = Registry.LocalMachine.CreateSubKey(UninstallKeyPath))
            {
                uninstallKey.SetValue("DisplayName", "Excel Entity Exporter");
                uninstallKey.SetValue("DisplayDescription", "Export Excel files by entity and month/year");
                uninstallKey.SetValue("UninstallString", $"\"{destExe}\" --uninstall");
                uninstallKey.SetValue("InstallLocation", installDir);
                uninstallKey.SetValue("Publisher", "ExcelEntityExporter");
            }

            MessageBox.Show(
                "Instalación completada correctamente.\n\n" +
                "Ahora puede hacer clic derecho en cualquier archivo .xlsx y seleccionar\n" +
                "\"Exportar por Entidad y Fecha\".",
                "Instalado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            UpdateStatus();
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show(
                "Se necesitan permisos de Administrador para instalar.\n" +
                "Por favor, haga clic derecho en el instalador y seleccione \"Ejecutar como administrador\".",
                "Permisos insuficientes",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error durante la instalación:\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void BtnUninstall_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            "¿Está seguro de que desea desinstalar Excel Entity Exporter?\n\n" +
            "Esto eliminará el menú contextual y el archivo instalado.",
            "Confirmar desinstalación",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes) return;

        try
        {
            // Remove context menu registry keys
            Registry.ClassesRoot.DeleteSubKeyTree(ContextMenuKey, throwOnMissingSubKey: false);

            // Remove uninstall entry
            Registry.LocalMachine.DeleteSubKeyTree(UninstallKeyPath, throwOnMissingSubKey: false);

            // Remove installed files
            string installDir = GetInstallPath();
            if (Directory.Exists(installDir))
            {
                Directory.Delete(installDir, recursive: true);
            }

            MessageBox.Show(
                "Desinstalación completada correctamente.",
                "Desinstalado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            UpdateStatus();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error durante la desinstalación:\n{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
