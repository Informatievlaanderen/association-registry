namespace AssociationRegistry.Scheduled.Host.PowerBi.Writers;

using CsvHelper;

public interface IRecordWriter<in TSource>
{
    Task Write(IEnumerable<TSource> docs, IWriter csvWriter);
}
