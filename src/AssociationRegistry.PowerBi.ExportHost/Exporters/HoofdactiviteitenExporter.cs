namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class HoofdactiviteitenExporter : IExporter
{

    public HoofdactiviteitenExporter()
    { }

    public async Task Export(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        foreach (var vereniging in docs)
        {
            foreach (var hoofdactiviteitVerenigingsloket in vereniging.HoofdactiviteitenVerenigingsloket)
            {
                csvWriter.WriteRecord(new HoofdactiviteitenRecord(
                                          hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Naam,
                                          vereniging.VCode));

                await csvWriter.NextRecordAsync();
            }
        }
    }
}
