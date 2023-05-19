namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Marten.Events.Aggregation;
using Schema.Detail;
using Vereniging;

public class PubliekVerenigingDetailProjection : SingleStreamAggregation<PubliekVerenigingDetailDocument>
{
    public PubliekVerenigingDetailDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Type = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = VerenigingsType.FeitelijkeVereniging.Code,
                Beschrijving = VerenigingsType.FeitelijkeVereniging.Beschrijving,
            },
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum,
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
            Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens.Select(
                c => new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = c.ContactgegevenId,
                    Type = c.Type.ToString(),
                    Waarde = c.Waarde,
                    Beschrijving = c.Beschrijving,
                    IsPrimair = c.IsPrimair,
                }).ToArray(),
            Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(MapLocatie).ToArray(),
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(MapHoofdactiviteit).ToArray(),
        };

    public PubliekVerenigingDetailDocument Create(IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Type = new PubliekVerenigingDetailDocument.VerenigingsType
            {
                Code = VerenigingsType.VerenigingMetRechtspersoonlijkheid.Code,
                Beschrijving = VerenigingsType.VerenigingMetRechtspersoonlijkheid.Beschrijving,
            },
            Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
            KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
            Contactgegevens = Array.Empty<PubliekVerenigingDetailDocument.Contactgegeven>(),
            Locaties = Array.Empty<PubliekVerenigingDetailDocument.Locatie>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket>(),
            Sleutels = new PubliekVerenigingDetailDocument.Sleutel[]
            {
                new()
                {
                    Bron = Bron.Kbo.Waarde,
                    Waarde = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
                },
            } };


    private static PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket MapHoofdactiviteit(FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket arg)
        => new()
        {
            Code = arg.Code,
            Beschrijving = arg.Beschrijving,
        };

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
        document.DatumLaatsteAanpassing = naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Startdatum = startdatumWerdGewijzigd.Data.Startdatum;
        document.DatumLaatsteAanpassing = startdatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
        document.DatumLaatsteAanpassing = korteNaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
        document.DatumLaatsteAanpassing = korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Append(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                    Type = contactgegevenWerdToegevoegd.Data.Type,
                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                    Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                })
            .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Where(c => c.ContactgegevenId != contactgegevenWerdGewijzigd.Data.ContactgegevenId)
            .Append(
                new PubliekVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Type = contactgegevenWerdGewijzigd.Data.Type,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                })
            .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, PubliekVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Where(c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
            .ToArray()
            .ToArray();

        document.DatumLaatsteAanpassing = contactgegevenWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket.Select(
            h => new PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
            {
                Code = h.Code,
                Beschrijving = h.Beschrijving,
            }).ToArray();
        document.DatumLaatsteAanpassing = hoofactiviteitenVerenigingloketWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    private static PubliekVerenigingDetailDocument.Locatie MapLocatie(FeitelijkeVerenigingWerdGeregistreerd.Locatie loc)
        => new()
        {
            Hoofdlocatie = loc.Hoofdlocatie,
            Naam = loc.Naam,
            Locatietype = loc.Locatietype,
            Straatnaam = loc.Straatnaam,
            Huisnummer = loc.Huisnummer,
            Busnummer = loc.Busnummer,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
            Land = loc.Land,
            Adres = loc.ToAdresString(),
        };
}
