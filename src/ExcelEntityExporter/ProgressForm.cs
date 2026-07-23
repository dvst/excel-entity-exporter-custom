namespace ExcelEntityExporter;

/// <summary>
/// Progress form shown during export. Displays a progress bar, status text,
/// and a cancel button. Runs the export on a background thread to keep the UI responsive.
/// </summary>
public partial class ProgressForm : Form
{
    private readonly string _filePath;
    private readonly CancellationTokenSource _cts = new();
    private Task<ExcelExporter.ExportResult>? _exportTask;

    public ProgressForm(string filePath)
    {
        InitializeComponent();
        _filePath = filePath;
        lblFileName.Text = Path.GetFileName(filePath);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        StartExport();
    }

    private void StartExport()
    {
        var progress = new Progress<int>(percent =>
        {
            if (IsHandleCreated && !IsDisposed)
            {
                progressBar.Value = Math.Min(percent, 100);
            }
        });

        _exportTask = Task.Run(() => ExcelExporter.Export(_filePath, progress, _cts.Token));

        _ = _exportTask.ContinueWith(t => OnExportComplete(t), TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void OnExportComplete(Task<ExcelExporter.ExportResult> task)
    {
        var result = task.Result;

        if (result.Success)
        {
            MessageBox.Show(
                $"Archivos generados correctamente en:\n{result.OutputFolder}\n\n" +
                $"Archivos creados: {result.FilesCreated}",
                "Exportación completada",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        else if (!string.IsNullOrEmpty(result.ErrorMessage) &&
                 result.ErrorMessage.Contains("cancelled", StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                "La exportación fue cancelada.",
                "Cancelado",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
        else
        {
            MessageBox.Show(
                result.ErrorMessage ?? "An unknown error occurred.",
                "Error en la exportación",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        Close();
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        _cts.Cancel();
        btnCancel.Enabled = false;
        lblStatus.Text = "Cancelando...";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_exportTask != null && !_exportTask.IsCompleted)
        {
            _cts.Cancel();
        }
        _cts.Dispose();
        base.OnFormClosing(e);
    }
}
