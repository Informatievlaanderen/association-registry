namespace AssociationRegistry.PowerBi.ExportHost.Exporters;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class LocatiesExporter
{
    private readonly CsvWriter _csvWriter;

    public LocatiesExporter(CsvWriter csvWriter)
    {
        _csvWriter = csvWriter;
    }

    public async Task Export(IEnumerable<PowerBiExportDocument> docs)
    {
        foreach (var vereniging in docs)
        {
            foreach (var locatie in vereniging.Locaties)
            {
                _csvWriter.WriteRecord(new LocatiesRecord(
                                           locatie.AdresId?.Broncode,
                                           locatie.AdresId?.Bronwaarde,
                                           locatie.Adresvoorstelling,
                                           locatie.Bron,
                                           locatie.Adres?.Busnummer,
                                           locatie.Adres?.Gemeente,
                                           locatie.Adres?.Huisnummer,
                                           locatie.IsPrimair,
                                           locatie.Adres?.Land,
                                           locatie.LocatieId,
                                           locatie.Locatietype,
                                           locatie.Naam,
                                           locatie.Adres?.Postcode,
                                           locatie.Adres?.Straatnaam,
                                           vereniging.VCode));

                await _csvWriter.NextRecordAsync();
            }
        }

    }

}
