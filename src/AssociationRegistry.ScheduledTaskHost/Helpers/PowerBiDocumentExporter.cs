namespace AssociationRegistry.ScheduledTaskHost.Helpers;

using Admin.Schema.PowerBiExport;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Text;

public class PowerBiDocumentExporter
{
    public async Task<MemoryStream> ExportAsync(IEnumerable<PowerBiExportDocument> docs)
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        await using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteHeader<HoofdactiviteitenRecord>();
        await csv.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var hoofdactiviteitVerenigingsloket in vereniging.HoofdactiviteitenVerenigingsloket)
            {
                csv.WriteRecord(new HoofdactiviteitenRecord(
                                    hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Naam,
                                    vereniging.VCode));

                await csv.NextRecordAsync();
            }
        }

        await csv.FlushAsync();

        memoryStream.Position = 0;

        var exportStream = new MemoryStream();
        await memoryStream.CopyToAsync(exportStream);

        exportStream.Position = 0;

        return exportStream;
    }

    public record HoofdactiviteitenRecord(
        [property: Name("code"), Index(0)] string Code,
        [property: Name("naam"), Index(1)] string Naam,
        [property: Name("vcode"), Index(2)] string VCode);

}
