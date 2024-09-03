namespace AssociationRegistry.PowerBi.ExportHost;

using Admin.Schema.PowerBiExport;
using CsvHelper;
using Records;
using System.Globalization;
using System.Text;

public class PowerBiDocumentExporter
{

    public PowerBiDocumentExporter()
    {
    }

    public async Task<MemoryStream> ExportHoofdactiviteiten(IEnumerable<PowerBiExportDocument> docs)
    {
        var hoofdactiviteitenSetup = await GetFileSetup<HoofdactiviteitenRecord>();

        foreach (var vereniging in docs)
        {
            foreach (var hoofdactiviteitVerenigingsloket in vereniging.HoofdactiviteitenVerenigingsloket)
            {
                hoofdactiviteitenSetup.CsvWriter.WriteRecord(new HoofdactiviteitenRecord(
                                                       hoofdactiviteitVerenigingsloket.Code, hoofdactiviteitVerenigingsloket.Naam,
                                                       vereniging.VCode));

                await hoofdactiviteitenSetup.CsvWriter.NextRecordAsync();
            }
        }

        return await CloseCsvAndCopyStream(hoofdactiviteitenSetup);
    }

    public async Task<MemoryStream> ExportBasisgegevens(IEnumerable<PowerBiExportDocument> docs)
    {
        var basisgegevensSetup = await GetFileSetup<BasisgegevensRecord>();

        foreach (var vereniging in docs)
        {
            basisgegevensSetup.CsvWriter.WriteRecord(new BasisgegevensRecord(
                                                         vereniging.Bron,
                                                         vereniging.Doelgroep.Maximumleeftijd,
                                                         vereniging.Doelgroep.Minimumleeftijd,
                                                         vereniging.Einddatum,
                                                         vereniging.IsUitgeschrevenUitPubliekeDatastroom,
                                                         vereniging.KorteBeschrijving,
                                                         vereniging.KorteNaam,
                                                         vereniging.Naam,
                                                         vereniging.Roepnaam,
                                                         vereniging.Startdatum,
                                                         vereniging.Status,
                                                         vereniging.VCode,
                                                         vereniging.Verenigingstype.Code,
                                                         vereniging.Verenigingstype.Naam,
                                                         vereniging.KboNummer,
                                                         string.Join(", ", vereniging.CorresponderendeVCodes),
                                                         vereniging.AantalVertegenwoordigers,
                                                         vereniging.DatumLaatsteAanpassing));

            await basisgegevensSetup.CsvWriter.NextRecordAsync();
        }

        return await CloseCsvAndCopyStream(basisgegevensSetup);
    }

    public async Task<MemoryStream> ExportLocaties(IEnumerable<PowerBiExportDocument> docs)
    {
        var basisgegevensSetup = await GetFileSetup<LocatiesRecord>();

        foreach (var vereniging in docs)
        {
            foreach (var locatie in vereniging.Locaties)
            {
                basisgegevensSetup.CsvWriter.WriteRecord(new LocatiesRecord(
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
                                                             locatie.Adres.Straatnaam,
                                                             vereniging.VCode));

                await basisgegevensSetup.CsvWriter.NextRecordAsync();
            }
        }

        return await CloseCsvAndCopyStream(basisgegevensSetup);
    }

    public record FileSetup(MemoryStream Stream, CsvWriter CsvWriter);

    private async Task<FileSetup> GetFileSetup<T>()
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream, Encoding.UTF8);
        var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteHeader<T>();
        await csv.NextRecordAsync();

        return new FileSetup(memoryStream, csv);
    }

    private async Task<MemoryStream> CloseCsvAndCopyStream(FileSetup fileSetup)
    {
        await fileSetup.CsvWriter.FlushAsync();

        fileSetup.Stream.Position = 0;

        var exportStream = new MemoryStream();
        await fileSetup.Stream.CopyToAsync(exportStream);

        exportStream.Position = 0;

        return exportStream;
    }
}
