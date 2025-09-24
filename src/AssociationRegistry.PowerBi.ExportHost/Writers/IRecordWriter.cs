namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using CsvHelper;

public interface IRecordWriter<in TSource>
{
    Task Write(IEnumerable<TSource> docs, IWriter csvWriter);
}
