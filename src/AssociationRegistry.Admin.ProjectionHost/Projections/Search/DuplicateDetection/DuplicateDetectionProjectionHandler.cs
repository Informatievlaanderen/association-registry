namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search.DuplicateDetection;

using DecentraalBeheer.Vereniging;
using Events;
using Formats;
using Hosts.Configuration.ConfigurationBindings;
using Schema.Search;
using Vereniging;

public class DuplicateDetectionProjectionHandler
{
    public DuplicateDetectionProjectionHandler()
    {
    }

    public void Handle(EventEnvelope<FeitelijkeVerenigingWerdGeregistreerd> message, DuplicateDetectionDocument document)
    {
        document.VCode = message.Data.VCode;
        document.VerenigingsTypeCode = Verenigingstype.FeitelijkeVereniging.Code;
        document.VerenigingssubtypeCode = null;
        document.Naam = message.Data.Naam;
        document.KorteNaam = message.Data.KorteNaam;
        document.Locaties = message.Data.Locaties.Select(Map).ToArray();
        document.HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket);
        document.IsGestopt = false;
        document.IsVerwijderd = false;
        document.IsDubbel = false;
    }

    public void Handle(
        EventEnvelope<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> message,
        DuplicateDetectionDocument document)
    {
        document.VCode = message.Data.VCode;
        document.VerenigingsTypeCode = Verenigingstype.VZER.Code;
        document.VerenigingssubtypeCode = VerenigingssubtypeCode.Default.Code;
        document.Naam = message.Data.Naam;
        document.KorteNaam = message.Data.KorteNaam;
        document.Locaties = message.Data.Locaties.Select(Map).ToArray();
        document.HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket);
        document.IsGestopt = false;
        document.IsVerwijderd = false;
        document.IsDubbel = false;
    }

    public void Handle(
        EventEnvelope<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> message,
        DuplicateDetectionDocument document)
    {
        document.VCode = message.Data.VCode;
        document.VerenigingsTypeCode = Verenigingstype.Parse(message.Data.Rechtsvorm).Code;
        document.VerenigingssubtypeCode = null;
        document.Naam = message.Data.Naam;
        document.KorteNaam = message.Data.KorteNaam;
        document.Locaties = [];
        document.HoofdactiviteitVerenigingsloket = [];
        document.IsGestopt = false;
        document.IsVerwijderd = false;
        document.IsDubbel = false;
    }

    public void Handle(EventEnvelope<NaamWerdGewijzigd> message, DuplicateDetectionDocument document)
    {
        document.Naam = message.Data.Naam;
    }

    public void Handle(EventEnvelope<RechtsvormWerdGewijzigdInKBO> message, DuplicateDetectionDocument document)
    {
        document.VerenigingsTypeCode = Verenigingstype.Parse(message.Data.Rechtsvorm).Code;
    }

    public void Handle(EventEnvelope<KorteNaamWerdGewijzigd> message, DuplicateDetectionDocument document)
    {
        document.KorteNaam = message.Data.KorteNaam;
    }

    public void Handle(EventEnvelope<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> message, DuplicateDetectionDocument document)
    {
        document.HoofdactiviteitVerenigingsloket = MapHoofdactiviteitVerenigingsloket(message.Data.HoofdactiviteitenVerenigingsloket);
    }

    public void Handle(EventEnvelope<LocatieWerdToegevoegd> message, DuplicateDetectionDocument document)
    {
        document.Locaties = document.Locaties.Append(Map(message.Data.Locatie))
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LocatieWerdGewijzigd> message, DuplicateDetectionDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .Append(Map(message.Data.Locatie))
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LocatieWerdVerwijderd> message, DuplicateDetectionDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<VerenigingWerdGestopt> message, DuplicateDetectionDocument document)
    {
        document.IsGestopt = true;
    }

    public void Handle(EventEnvelope<VerenigingWerdGestoptInKBO> message, DuplicateDetectionDocument document)
    {
        document.IsGestopt = true;
    }

    public void Handle(EventEnvelope<VerenigingWerdVerwijderd> message, DuplicateDetectionDocument document)
    {
        document.IsVerwijderd = true;
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdOvergenomenUitKbo> message, DuplicateDetectionDocument document)
    {
        document.Locaties = document.Locaties.Append(Map(message.Data.Locatie))
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdGewijzigdInKbo> message, DuplicateDetectionDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .Append(Map(message.Data.Locatie))
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelWerdVerwijderdUitKbo> message, DuplicateDetectionDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.Locatie.LocatieId)
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> message, DuplicateDetectionDocument document)
    {

        var maatschappelijkeZetel = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        maatschappelijkeZetel.LocatieId = message.Data.LocatieId;
        maatschappelijkeZetel.Naam = message.Data.Naam;
        maatschappelijkeZetel.IsPrimair = message.Data.IsPrimair;

        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.LocatieId)
                                    .Append(maatschappelijkeZetel)
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    private static DuplicateDetectionDocument.Locatie Map(Registratiedata.Locatie locatie)
        => new()
        {
            LocatieId = locatie.LocatieId,
            Locatietype = locatie.Locatietype,
            Naam = locatie.Naam,
            Adresvoorstelling = locatie.Adres.ToAdresString(),
            IsPrimair = locatie.IsPrimair,
            Postcode = locatie.Adres?.Postcode ?? string.Empty,
            Gemeente = locatie.Adres?.Gemeente ?? string.Empty,
        };

    public void Handle(EventEnvelope<NaamWerdGewijzigdInKbo> message, DuplicateDetectionDocument document)
    {
        document.Naam = message.Data.Naam;
    }

    public void Handle(EventEnvelope<KorteNaamWerdGewijzigdInKbo> message, DuplicateDetectionDocument document)
    {
        document.KorteNaam = message.Data.KorteNaam;
    }

    public void Handle(EventEnvelope<AdresWerdOvergenomenUitAdressenregister> message, DuplicateDetectionDocument document)
    {
        var locatie = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        locatie.LocatieId = message.Data.LocatieId;
        locatie.Adresvoorstelling = message.Data.Adres.ToAdresString();
        locatie.Gemeente = message.Data.Adres.Gemeente;
        locatie.Postcode = message.Data.Adres.Postcode;

        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.LocatieId)
                                    .Append(locatie)
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<AdresWerdGewijzigdInAdressenregister> message, DuplicateDetectionDocument document)
    {
        var locatie = document.Locaties.Single(x => x.LocatieId == message.Data.LocatieId);

        locatie.LocatieId = message.Data.LocatieId;
        locatie.Adresvoorstelling = message.Data.Adres.ToAdresString();
        locatie.Gemeente = message.Data.Adres.Gemeente;
        locatie.Postcode = message.Data.Adres.Postcode;

        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.LocatieId)
                                    .Append(locatie)
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<LocatieDuplicaatWerdVerwijderdNaAdresMatch> message, DuplicateDetectionDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(x => x.LocatieId != message.Data.VerwijderdeLocatieId)
                                    .OrderBy(x => x.LocatieId)
                                    .ToArray();
    }

    public void Handle(EventEnvelope<VerenigingWerdGemarkeerdAlsDubbelVan> message, DuplicateDetectionDocument document)
    {
        document.IsDubbel = true;
    }

    public void Handle(
        EventEnvelope<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> message,
        DuplicateDetectionDocument document)
    {
        document.IsDubbel = false;
    }

    public void Handle(EventEnvelope<MarkeringDubbeleVerengingWerdGecorrigeerd> message, DuplicateDetectionDocument document)
    {
        document.IsDubbel = false;
    }

    public void Handle(
        EventEnvelope<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> message,
        DuplicateDetectionDocument document)
    {
        document.VerenigingsTypeCode = Verenigingstype.VZER.Code;
        document.VerenigingssubtypeCode = VerenigingssubtypeCode.Default.Code;
    }

    public void Handle(
        EventEnvelope<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging> message,
        DuplicateDetectionDocument document)
    {
        document.VerenigingssubtypeCode = VerenigingssubtypeCode.FeitelijkeVereniging.Code;
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdTerugGezetNaarNietBepaald> message, DuplicateDetectionDocument document)
    {
        document.VerenigingssubtypeCode = VerenigingssubtypeCode.NietBepaald.Code;
    }

    public void Handle(EventEnvelope<VerenigingssubtypeWerdVerfijndNaarSubvereniging> message, DuplicateDetectionDocument document)
    {
        document.VerenigingssubtypeCode = VerenigingssubtypeCode.Subvereniging.Code;
    }

    private static string[] MapHoofdactiviteitVerenigingsloket(
        IEnumerable<Registratiedata.HoofdactiviteitVerenigingsloket> hoofdactiviteitenVerenigingsloket)
    {
        return hoofdactiviteitenVerenigingsloket.Select(x => x.Code).ToArray();
    }
}
