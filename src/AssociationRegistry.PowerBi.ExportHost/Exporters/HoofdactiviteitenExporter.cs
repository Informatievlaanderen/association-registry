namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class HoofdactiviteitenExporter
{
    private readonly CsvWriter _csvWriter;

    public HoofdactiviteitenExporter(CsvWriter csvWriter)
    {
        _csvWriter = csvWriter;
    }

    public async Task Export(IEnumerable<PowerBiExportDocument> docs)
    {
        foreach (var vereniging in docs)
        {
            foreach (var hoofdactiviteitVerenigingsloket in vereniging.HoofdactiviteitenVerenigingsloket)
            {
                _csvWriter.WriteRecord(new HoofdactiviteitenRecord(
                                           hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Naam,
                                           vereniging.VCode));

                await _csvWriter.NextRecordAsync();
            }
        }
    }
}
