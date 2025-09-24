namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using CsvHelper;
using Records.DubbelDetectie;

public class DubbelDetectieRecordWriter: IRecordWriter<PowerBiExportDubbelDetectieDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDubbelDetectieDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<DubbelDetectieRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var doc in docs)
        {
            csvWriter.WriteRecord(new DubbelDetectieRecord(
                                      doc.Id,
                                      doc.Bevestigingstoken,
                                      doc.Naam,
                                      string.Join(", ", doc.Locaties.Select(x => x.Adres?.Postcode)),
                                      string.Join(", ", doc.Locaties.Select(x => x.Adres?.Gemeente)),
                                      string.Join(", ", doc.GedetecteerdeDubbels.Select(x => x.VCode)),
                                      doc.Tijdstip,
                                      doc.Initiator,
                                      doc.CorrelationId));

            await csvWriter.NextRecordAsync();
        }
    }
}
