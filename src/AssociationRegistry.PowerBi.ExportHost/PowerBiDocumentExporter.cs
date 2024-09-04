namespace AssociationRegistry.PowerBi.ExportHost;

using Admin.Schema.PowerBiExport;
using CsvHelper;
using Exporters;
using System.Globalization;
using System.Text;

public class PowerBiDocumentExporter
{
    public async Task<MemoryStream> Export(IEnumerable<PowerBiExportDocument> docs, IRecordWriter recordWriter)
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        await recordWriter.Write(docs, csvWriter);
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
