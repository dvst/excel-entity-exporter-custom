namespace ExcelEntityExporter;

partial class InstallerForm
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
        this.lblTitle = new Label();
        this.lblVersion = new Label();
        this.lblDescription = new Label();
        this.lblStatus = new Label();
        this.btnInstall = new Button();
        this.btnUninstall = new Button();
        this.SuspendLayout();

        // lblTitle
        this.lblTitle.AutoSize = true;
        this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        this.lblTitle.Location = new Point(30, 20);
        this.lblTitle.Text = "Excel Entity Exporter";

        // lblVersion
        this.lblVersion.AutoSize = true;
        this.lblVersion.Font = new Font("Segoe UI", 9F);
        this.lblVersion.ForeColor = Color.Gray;
        this.lblVersion.Location = new Point(30, 48);
        this.lblVersion.Text = "";

        // lblDescription
        this.lblDescription.Font = new Font("Segoe UI", 9F);
        this.lblDescription.Location = new Point(30, 75);
        this.lblDescription.Size = new Size(340, 30);
        this.lblDescription.Text = "Adds \"Exportar por Entidad y Fecha\" to the right-click " +
            "context menu on .xlsx files in Windows Explorer.";

        // lblStatus
        this.lblStatus.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
        this.lblStatus.ForeColor = Color.Gray;
        this.lblStatus.Location = new Point(30, 115);
        this.lblStatus.Size = new Size(340, 20);
        this.lblStatus.Text = "";

        // btnInstall
        this.btnInstall.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        this.btnInstall.BackColor = Color.FromArgb(0, 120, 215);
        this.btnInstall.ForeColor = Color.White;
        this.btnInstall.FlatStyle = FlatStyle.Flat;
        this.btnInstall.Location = new Point(30, 150);
        this.btnInstall.Size = new Size(160, 40);
        this.btnInstall.Text = "Instalar";
        this.btnInstall.UseVisualStyleBackColor = false;
        this.btnInstall.Click += new EventHandler(this.BtnInstall_Click);

        // btnUninstall
        this.btnUninstall.Font = new Font("Segoe UI", 10F);
        this.btnUninstall.FlatStyle = FlatStyle.Flat;
        this.btnUninstall.Location = new Point(210, 150);
        this.btnUninstall.Size = new Size(160, 40);
        this.btnUninstall.Text = "Desinstalar";
        this.btnUninstall.Click += new EventHandler(this.BtnUninstall_Click);

        // InstallerForm
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(400, 210);
        this.Controls.Add(this.lblTitle);
        this.Controls.Add(this.lblVersion);
        this.Controls.Add(this.lblDescription);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.btnInstall);
        this.Controls.Add(this.btnUninstall);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Excel Entity Exporter";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private Label lblTitle;
    private Label lblVersion;
    private Label lblDescription;
    private Label lblStatus;
    private Button btnInstall;
    private Button btnUninstall;
}
