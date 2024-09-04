namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using CsvHelper;

public interface IExporter
{
    Task Export(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter);
}
