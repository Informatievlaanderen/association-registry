namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using CsvHelper;

public interface IRecordWriter
{
    Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter);
}
