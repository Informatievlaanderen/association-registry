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

        foreach (var vereniging in docs)
        {
            csvWriter.WriteRecord(new DubbelDetectieRecord(
                                      vereniging.BevestigingstokenKey,
                                      vereniging.Bevestigingstoken,
                                      vereniging.Naam,
                                      string.Join(", ", vereniging.Locaties.Select(x => x.Adres?.Postcode)),
                                      string.Join(", ", vereniging.Locaties.Select(x => x.Adres?.Gemeente)),
                                      string.Join(", ", vereniging.GedetecteerdeDubbels.Select(x => x.VCode))));

            await csvWriter.NextRecordAsync();
        }
    }
}
