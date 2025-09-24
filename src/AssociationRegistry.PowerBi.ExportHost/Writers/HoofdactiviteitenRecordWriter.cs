namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class HoofdactiviteitenRecordWriter: IRecordWriter<PowerBiExportDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<HoofdactiviteitenRecord>();
        await csvWriter.NextRecordAsync();

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
