namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;
using Formats;
using NodaTime;

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
