# Excel Entity Exporter

[![Build Installer](https://github.com/dvst/excel-entity-exporter-custom/actions/workflows/build.yml/badge.svg)](https://github.com/dvst/excel-entity-exporter-custom/actions/workflows/build.yml)
[![Latest Release](https://img.shields.io/github/v/release/dvst/excel-entity-exporter-custom?label=latest%20release)](https://github.com/dvst/excel-entity-exporter-custom/releases/latest)

**Español:** [Lee este README en español](README.es.md)

---

A Windows tool that adds **"Exportar por Entidad y Fecha"** to the right-click context menu on `.xlsx` files. It reads an Excel spreadsheet, groups rows by entity and coverage start date (month/year), and exports each group to a separate file.

## Demo

1. Right-click any `.xlsx` file in Windows Explorer
2. Select **"Exportar por Entidad y Fecha"**
3. A progress bar appears while files are being generated
4. A new folder `Archivos_por_Entidad` is created next to the original file with the exported files

## Installation

### Download

Download the latest version from the [**Releases**](https://github.com/dvst/excel-entity-exporter-custom/releases/latest) page. You will get a single file: `installer-entity-exporter.exe`.

### Install

1. Double-click `installer-entity-exporter.exe`
2. Click **Instalar**
3. Done! The context menu option is now available on all `.xlsx` files

> **Note:** You may need to run the installer as Administrator if you encounter permission errors.

### Uninstall

1. Double-click `installer-entity-exporter.exe` again
2. Click **Desinstalar**
3. The context menu option and installed files are removed

The application also appears in **Windows Settings → Apps** where you can uninstall it.

## Usage

### Export files by entity and date

1. Open Windows Explorer and navigate to your `.xlsx` file
2. Right-click the file
3. Select **"Exportar por Entidad y Fecha"**
4. Wait for the progress bar to complete
5. A folder named `Archivos_por_Entidad` is created in the same location as the original file

### File naming

Exported files follow this pattern:

```
IND [MONTH] [YEAR] [ENTITY NAME].xlsx
```

For example:
- `IND ENERO 2025 MiEmpresa.xlsx`
- `IND JUNIO 2025 OtraEmpresa.xlsx`

### Required columns

Your Excel file must have these columns (header row):

| Column | Description |
|---|---|
| `RAZON SOCIAL` or `ENTIDAD` | Company/entity name |
| `FECHA INICIO COBERTURA` | Coverage start date (must be a proper date, not text) |

Any additional columns in your spreadsheet are preserved in the exported files.

## Frequently Asked Questions

**Q: Do I need Microsoft Excel installed?**
A: No. The application works independently and does not require Excel to be installed.

**Q: What happens if the date column contains text instead of dates?**
A: Rows with text dates are skipped. Make sure the "FECHA INICIO COBERTURA" column contains actual Excel date values.

**Q: Can I export the same file multiple times?**
A: Yes. Each time you run the export, the files in the output folder are overwritten.

**Q: Does it work with `.xls` files (old Excel format)?**
A: Currently only `.xlsx` files are supported.

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to contribute to this project.

## License

This project is open source. See the repository for license details.
