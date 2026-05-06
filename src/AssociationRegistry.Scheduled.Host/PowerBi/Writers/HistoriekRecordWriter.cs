namespace AssociationRegistry.Scheduled.Host.PowerBi.Writers;

using AssociationRegistry.Admin.Schema.PowerBiExport;
using AssociationRegistry.Formats;
using CsvHelper;
using NodaTime;
using Records;

public class HistoriekRecordWriter: IRecordWriter<PowerBiExportDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<HistoriekRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            foreach (var gebeurtenis in vereniging.Historiek)
            {
                var gebeurtenisDatum = Instant.FromDateTimeOffset(DateTimeOffset.Parse(gebeurtenis.Datum));
                csvWriter.WriteRecord(new HistoriekRecord(
                                          gebeurtenisDatum.ConvertAndFormatToBelgianDate(),
                                          gebeurtenis.EventType,
                                          gebeurtenis.Initiator,
                                          gebeurtenisDatum.ConvertAndFormatToBelgianTime(),
                                          vereniging.VCode,
                                          gebeurtenis.Sequence));

                await csvWriter.NextRecordAsync();
            }
        }
    }
}
