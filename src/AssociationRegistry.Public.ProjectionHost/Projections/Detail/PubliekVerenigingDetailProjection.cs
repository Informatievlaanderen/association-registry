namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using System;
using Events;
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
            DatumLaatsteAanpassing = verenigingWerdGeregistreerd.Data.DatumLaatsteAanpassing ?? DateOnly.Parse(verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)),
            Status = "Actief",
            ContactInfoLijst = verenigingWerdGeregistreerd.Data.ContactInfoLijst?.Select(
                            c => new PubliekVerenigingDetailDocument.ContactInfo()
                            {
                                Contactnaam = c.Contactnaam,
                                Email = c.Email,
                                Telefoon = c.Telefoon,
                                Website = c.Website,
                                SocialMedia = c.SocialMedia,
                            }).ToArray()
                        ?? Array.Empty<PubliekVerenigingDetailDocument.ContactInfo>(),
            Locaties = verenigingWerdGeregistreerd.Data.Locaties?.Select(MapLocatie).ToArray() ?? Array.Empty<PubliekVerenigingDetailDocument.Locatie>(),
        };

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
    }

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, PubliekVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
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
