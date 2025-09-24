namespace AssociationRegistry.Admin.ProjectionHost.Projections.PowerBiExport;

using Events;
using Formats;
using JasperFx.Events;
using Marten.Events.Aggregation;
using Schema.PowerBiExport;

public class PowerBiExportDubbelDetectieProjection : SingleStreamProjection<PowerBiExportDubbelDetectieDocument, string>
{
    public PowerBiExportDubbelDetectieDocument Create(IEvent<DubbeleVerenigingenWerdenGedetecteerd> dubbeleVerenigingenWerdenGedetecteerd)
    {
        var document = new PowerBiExportDubbelDetectieDocument()
        {
            BevestigingstokenKey = dubbeleVerenigingenWerdenGedetecteerd.Data.BevestigingstokenKey,
            Bevestigingstoken = dubbeleVerenigingenWerdenGedetecteerd.Data.Bevestigingstoken,
            Naam = dubbeleVerenigingenWerdenGedetecteerd.Data.Naam,
            Locaties = dubbeleVerenigingenWerdenGedetecteerd.Data.Locaties.Select(MapLocatie).ToArray(),
            GedetecteerdeDubbels = dubbeleVerenigingenWerdenGedetecteerd.Data.GedetecteerdeDubbels.Select(MapDuplicate).ToArray(),
        };
        return document;
    }


    public static PowerBiExportDubbelDetectieDocument.Types.DuplicateVereniging MapDuplicate(
        Registratiedata.DuplicateVereniging src)
    {
        return new PowerBiExportDubbelDetectieDocument.Types.DuplicateVereniging
        {
            VCode = src.VCode,
            Verenigingstype = new PowerBiExportDubbelDetectieDocument.Types.Verenigingstype(
                src.Verenigingstype.Code, src.Verenigingstype.Naam),

            Verenigingssubtype = src.Verenigingssubtype is null
                ? new PowerBiExportDubbelDetectieDocument.Types.Verenigingssubtype(string.Empty, string.Empty)
                : new PowerBiExportDubbelDetectieDocument.Types.Verenigingssubtype(
                    src.Verenigingssubtype.Code, src.Verenigingssubtype.Naam),

            Naam = src.Naam,
            KorteNaam = src.KorteNaam,

            HoofdactiviteitenVerenigingsloket = src.HoofdactiviteitenVerenigingsloket
                                               .Select(h => new PowerBiExportDubbelDetectieDocument.Types.HoofdactiviteitVerenigingsloket
                                                {
                                                    Code = h.Code,
                                                    Naam = h.Naam
                                                })
                                               .ToArray(),

            Locaties = src.Locaties
                      .Select(MapDuplicateLocatie)
                      .ToArray()
        };
    }

    public static PowerBiExportDubbelDetectieDocument.Types.DuplicateVerenigingLocatie MapDuplicateLocatie(
        Registratiedata.DuplicateVerenigingLocatie src)
        => new(
            src.Locatietype,
            src.IsPrimair,
            src.Adres,
            src.Naam,
            src.Postcode,
            src.Gemeente
        );

    public static PowerBiExportDubbelDetectieDocument.Types.Locatie MapLocatie(Registratiedata.Locatie loc)
        => new()
        {
            IsPrimair = loc.IsPrimair,
            Naam = loc.Naam,
            Locatietype = loc.Locatietype,
            Adres = MapAdres(loc.Adres),
            Adresvoorstelling = loc.Adres.ToAdresString(),
            AdresId = MapAdresId(loc.AdresId),
        };

    public static PowerBiExportDubbelDetectieDocument.Types.Adres? MapAdres(Registratiedata.Adres? adres)
        => adres is null
            ? null
            : new PowerBiExportDubbelDetectieDocument.Types.Adres
            {
                Straatnaam = adres.Straatnaam,
                Huisnummer = adres.Huisnummer,
                Busnummer = adres.Busnummer,
                Postcode = adres.Postcode,
                Gemeente = adres.Gemeente,
                Land = adres.Land,
            };

    public static PowerBiExportDubbelDetectieDocument.Types.AdresId? MapAdresId(Registratiedata.AdresId? locAdresId)
        => locAdresId is null
            ? null
            : new PowerBiExportDubbelDetectieDocument.Types.AdresId
            {
                Bronwaarde = locAdresId.Bronwaarde,
                Broncode = locAdresId.Broncode,
            };
}
