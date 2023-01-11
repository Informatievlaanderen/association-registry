namespace AssociationRegistry.Public.ProjectionHost.Projections.Detail;

using System;
using Framework;
using Infrastructure.Extensions;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;
using Schema.Detail;
using Vereniging;

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

    private static PubliekVerenigingDetailDocument.Locatie MapLocatie(VerenigingWerdGeregistreerd.Locatie loc)
        => new()
        {
            Hoofdlocatie = loc.Hoofdlocatie,
            Naam = loc.Naam,
            Type = loc.LocatieType,
            Straatnaam = loc.Straatnaam,
            Huisnummer = loc.Huisnummer,
            Busnummer = loc.Busnummer,
            Postcode = loc.Postcode,
            Gemeente = loc.Gemeente,
            Land = loc.Land,
            Adres = loc.ToAdresString(),
        };
}
