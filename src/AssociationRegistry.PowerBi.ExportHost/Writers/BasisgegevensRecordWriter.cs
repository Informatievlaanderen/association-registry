namespace AssociationRegistry.PowerBi.ExportHost.Writers;

using Admin.Schema.PowerBiExport;
using Records;
using CsvHelper;

public class BasisgegevensRecordWriter: IRecordWriter<PowerBiExportDocument>
{
    public async Task Write(IEnumerable<PowerBiExportDocument> docs, IWriter csvWriter)
    {
        csvWriter.WriteHeader<BasisgegevensRecord>();
        await csvWriter.NextRecordAsync();

        foreach (var vereniging in docs)
        {
            csvWriter.WriteRecord(new BasisgegevensRecord(
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
                                       vereniging.DatumLaatsteAanpassing,
                                       vereniging.Verenigingssubtype?.Code ?? string.Empty,
                                       vereniging.Verenigingssubtype?.Naam ?? string.Empty,
                                       vereniging.SubverenigingVan?.AndereVereniging ?? string.Empty,
                                       vereniging.SubverenigingVan?.Identificatie ?? string.Empty,
                                       vereniging.SubverenigingVan?.Beschrijving ?? string.Empty,
                                       vereniging.DuplicatieInfo?.BevestigdNaDuplicatie,
                                       vereniging.DuplicatieInfo?.Bevestigingstoken ?? string.Empty
                                       ));

            await csvWriter.NextRecordAsync();
        }
    }
}
