namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class HistoriekRecordWriter : IRecordWriter
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<HistoriekRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var gebeurtenis in vereniging.Historiek)
            {
                csvWriter.WriteRecord(new HistoriekRecord(
                                           gebeurtenis.Datum,
                                           gebeurtenis.EventType,
                                           gebeurtenis.Initiator,
                                           gebeurtenis.Tijdstip,
                                           vereniging.VCode,
                                           gebeurtenis.Sequence));

                await csvWriter.NextRecordAsync();
            }
        }

    }

}
