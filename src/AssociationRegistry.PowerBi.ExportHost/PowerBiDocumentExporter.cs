namespace AssociationRegistry.PowerBi.ExportHost;

using Admin.Schema.PowerBiExport;
using CsvHelper;
using Exporters;
using Records;
using System.Globalization;
using System.Text;

public class PowerBiDocumentExporter
{
    public PowerBiDocumentExporter()
    {
    }

    public async Task<MemoryStream> ExportHoofdactiviteiten(IEnumerable<PowerBiExportDocument> docs)
    {
        var fileSetup = await GetFileSetup<HoofdactiviteitenRecord>();
        var exporter = new HoofdactiviteitenExporter(fileSetup.CsvWriter);
        await exporter.Export(docs);
        return await CloseCsvAndCopyStream(fileSetup);
    }

    public async Task<MemoryStream> ExportBasisgegevens(IEnumerable<PowerBiExportDocument> docs)
    {
        var fileSetup = await GetFileSetup<BasisgegevensRecord>();
        var exporter = new BasisgegevensExporter(fileSetup.CsvWriter);
        await exporter.Export(docs);
        return await CloseCsvAndCopyStream(fileSetup);
    }

    public async Task<MemoryStream> ExportLocaties(IEnumerable<PowerBiExportDocument> docs)
    {
        var fileSetup = await GetFileSetup<LocatiesRecord>();
        var exporter = new LocatiesExporter(fileSetup.CsvWriter);
        await exporter.Export(docs);
        return await CloseCsvAndCopyStream(fileSetup);
    }

    public async Task<MemoryStream> ExportContactgegevens(IEnumerable<PowerBiExportDocument> docs)
    {
        var fileSetup = await GetFileSetup<ContactgegevensRecord>();
        var exporter = new ContactgegevensExporter(fileSetup.CsvWriter);
        await exporter.Export(docs);
        return await CloseCsvAndCopyStream(fileSetup);
    }

    public record FileSetup(MemoryStream Stream, CsvWriter CsvWriter);

    private async Task<FileSetup> GetFileSetup<T>()
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteHeader<T>();
        await csv.NextRecordAsync();

        return new FileSetup(memoryStream, csv);
    }

    private async Task<MemoryStream> CloseCsvAndCopyStream(FileSetup fileSetup)
    {
        await fileSetup.CsvWriter.FlushAsync();

        fileSetup.Stream.Position = 0;

        var exportStream = new MemoryStream();
        await fileSetup.Stream.CopyToAsync(exportStream);

        exportStream.Position = 0;

        return exportStream;
    }
}
