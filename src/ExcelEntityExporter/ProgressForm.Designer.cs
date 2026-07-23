namespace ExcelEntityExporter;

partial class ProgressForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.progressBar = new ProgressBar();
        this.lblStatus = new Label();
        this.lblFileName = new Label();
        this.btnCancel = new Button();
        this.SuspendLayout();

        // progressBar
        this.progressBar.Location = new Point(20, 50);
        this.progressBar.Size = new Size(440, 25);
        this.progressBar.Style = ProgressBarStyle.Continuous;
        this.progressBar.Minimum = 0;
        this.progressBar.Maximum = 100;

        // lblStatus
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.lblStatus.Location = new Point(20, 15);
        this.lblStatus.Text = "Exportando archivos...";

        // lblFileName
        this.lblFileName.AutoSize = true;
        this.lblFileName.Font = new Font("Segoe UI", 8F);
        this.lblFileName.ForeColor = Color.Gray;
        this.lblFileName.Location = new Point(20, 85);
        this.lblFileName.Size = new Size(440, 13);
        this.lblFileName.Text = "";

        // btnCancel
        this.btnCancel.Font = new Font("Segoe UI", 9F);
        this.btnCancel.Location = new Point(190, 110);
        this.btnCancel.Size = new Size(100, 30);
        this.btnCancel.Text = "Cancelar";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new EventHandler(this.BtnCancel_Click);

        // ProgressForm
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(480, 155);
        this.Controls.Add(this.progressBar);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.lblFileName);
        this.Controls.Add(this.btnCancel);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Exportar por Entidad y Fecha";
        this.ControlBox = false;
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private ProgressBar progressBar;
    private Label lblStatus;
    private Label lblFileName;
    private Button btnCancel;
}
