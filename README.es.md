# Excel Entity Exporter

[![Build Installer](https://github.com/dvst/excel-entity-exporter-custom/actions/workflows/build.yml/badge.svg)](https://github.com/dvst/excel-entity-exporter-custom/actions/workflows/build.yml)
[![Último release](https://img.shields.io/github/v/release/dvst/excel-entity-exporter-custom?label=último%20release)](https://github.com/dvst/excel-entity-exporter-custom/releases/latest)

**English:** [Read this README in English](README.md)

---

Una herramienta para Windows que agrega **"Exportar por Entidad y Fecha"** al menú contextual al hacer clic derecho en archivos `.xlsx`. Lee una hoja de Excel, agrupa filas por entidad y fecha de inicio de cobertura (mes/año), y exporta cada grupo a un archivo independiente.

## Instalación

### Descarga

Descarga la última versión desde la página de [**Releases**](https://github.com/dvst/excel-entity-exporter-custom/releases/latest). Obtendrás un solo archivo: `installer-entity-exporter.exe`.

### Instalar

1. Haz doble clic en `installer-entity-exporter.exe`
2. Haz clic en **Instalar**
3. ¡Listo! La opción del menú contextual ahora está disponible en todos los archivos `.xlsx`

> **Nota:** Es posible que necesites ejecutar el instalador como Administrador si encuentras errores de permisos.

### Desinstalar

1. Haz doble clic en `installer-entity-exporter.exe` nuevamente
2. Haz clic en **Desinstalar**
3. La opción del menú contextual y los archivos instalados se eliminan

La aplicación también aparece en **Configuración de Windows → Aplicaciones** donde puedes desinstalarla.

## Uso

### Exportar archivos por entidad y fecha

1. Abre el Explorador de Windows y navega hasta tu archivo `.xlsx`
2. Haz clic derecho en el archivo
3. Selecciona **"Exportar por Entidad y Fecha"**
4. Espera a que la barra de progreso termine
5. Se crea una carpeta llamada `Archivos_por_Entidad` en la misma ubicación que el archivo original

### Nombre de los archivos

Los archivos exportados siguen este patrón:

```
IND [MES] [AÑO] [NOMBRE ENTIDAD].xlsx
```

Por ejemplo:
- `IND ENERO 2025 MiEmpresa.xlsx`
- `IND JUNIO 2025 OtraEmpresa.xlsx`

### Columnas requeridas

Tu archivo de Excel debe tener estas columnas (fila de encabezados):

| Columna | Descripción |
|---|---|
| `RAZON SOCIAL` o `ENTIDAD` | Nombre de la empresa/entidad |
| `FECHA INICIO COBERTURA` | Fecha de inicio de cobertura (debe ser una fecha real de Excel, no texto) |

Cualquier columna adicional en tu hoja de cálculo se conserva en los archivos exportados.

## Preguntas frecuentes

**P: ¿Necesito tener Microsoft Excel instalado?**
R: No. La aplicación funciona de forma independiente y no requiere que Excel esté instalado.

**P: ¿Qué pasa si la columna de fechas contiene texto en lugar de fechas?**
R: Las filas con fechas en formato texto se omiten. Asegúrate de que la columna "FECHA INICIO COBERTURA" contenga valores de fecha reales de Excel.

**P: ¿Puedo exportar el mismo archivo varias veces?**
R: Sí. Cada vez que ejecutas la exportación, los archivos en la carpeta de salida se sobrescriben.

**P: ¿Funciona con archivos `.xls` (formato antiguo de Excel)?**
R: Actualmente solo se admiten archivos `.xlsx`.

## Contribuir

Consulta [CONTRIBUTING.md](CONTRIBUTING.md) para obtener pautas sobre cómo contribuir a este proyecto.

## Licencia

Este proyecto es de código abierto. Consulta el repositorio para obtener detalles de la licencia.
