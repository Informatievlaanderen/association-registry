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
        => await ExportFile(docs, new HoofdactiviteitenExporter());

    public async Task<MemoryStream> ExportBasisgegevens(IEnumerable<PowerBiExportDocument> docs)
        => await ExportFile(docs, new BasisgegevensExporter());

    public async Task<MemoryStream> ExportLocaties(IEnumerable<PowerBiExportDocument> docs)
        => await ExportFile(docs, new LocatiesExporter());

    public async Task<MemoryStream> ExportContactgegevens(IEnumerable<PowerBiExportDocument> docs)
        => await ExportFile(docs, new ContactgegevensExporter());

    private async Task<MemoryStream> ExportFile(IEnumerable<PowerBiExportDocument> docs, IExporter exporter)
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await exporter.Export(docs, csvWriter);
        await csvWriter.FlushAsync();

        return await CopyStream(memoryStream);
    }

    private static async Task<MemoryStream> CopyStream(MemoryStream memoryStream)
    {
        memoryStream.Position = 0;

        var exportStream = new MemoryStream();
        await memoryStream.CopyToAsync(exportStream);
        exportStream.Position = 0;

        return exportStream;
    }
}
