namespace AssociationRegistry.Scheduled.Host.PowerBi.Writers;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using CsvHelper;
using Records;

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
