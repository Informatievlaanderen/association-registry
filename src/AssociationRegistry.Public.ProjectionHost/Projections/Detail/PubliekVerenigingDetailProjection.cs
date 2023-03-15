namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using Events;
using Events.CommonEventDataTypes;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Marten.Events.Aggregation;
using Schema.Detail;

public class PubliekVerenigingDetailProjection : SingleStreamAggregation<PubliekVerenigingDetailDocument>
{
    public PubliekVerenigingDetailDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
        => new()
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Naam = verenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = verenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = verenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = verenigingWerdGeregistreerd.Data.Startdatum,
            KboNummer = verenigingWerdGeregistreerd.Data.KboNummer,
            DatumLaatsteAanpassing = verenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
            Status = "Actief",
            ContactInfoLijst = verenigingWerdGeregistreerd.Data.ContactInfoLijst.Select(
                ToDetailContactInfo).ToArray(),
            Locaties = verenigingWerdGeregistreerd.Data.Locaties.Select(MapLocatie).ToArray(),
            HoofdactiviteitenVerenigingsloket = verenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(MapHoofdactiviteit).ToArray(),
        };

    private static PubliekVerenigingDetailDocument.ContactInfo ToDetailContactInfo(ContactInfo c)
        => new()
        {
            Contactnaam = c.Contactnaam,
            Email = c.Email,
            Telefoon = c.Telefoon,
            Website = c.Website,
            SocialMedia = c.SocialMedia,
            PrimairContactInfo = c.PrimairContactInfo,
        };

    private static PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket MapHoofdactiviteit(VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket arg)
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

    public void Apply(IEvent<ContactInfoLijstWerdGewijzigd> contactInfoWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.ContactInfoLijst = document.ContactInfoLijst
            .Concat(contactInfoWerdGewijzigd.Data.Toevoegingen.Select(ToDetailContactInfo))
            .ExceptBy(contactInfoWerdGewijzigd.Data.Verwijderingen.Select(info => info.Contactnaam), info => info.Contactnaam)
            .ExceptBy(contactInfoWerdGewijzigd.Data.Wijzigingen.Select(info => info.Contactnaam), info => info.Contactnaam)
            .Concat(contactInfoWerdGewijzigd.Data.Wijzigingen.Select(ToDetailContactInfo))
            .ToArray();

        document.DatumLaatsteAanpassing = contactInfoWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }


    private static PubliekVerenigingDetailDocument.Locatie MapLocatie(VerenigingWerdGeregistreerd.Locatie loc)
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
