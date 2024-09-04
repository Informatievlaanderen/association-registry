namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using CsvHelper;

public interface IRecordWriter
{
    Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter);
}
