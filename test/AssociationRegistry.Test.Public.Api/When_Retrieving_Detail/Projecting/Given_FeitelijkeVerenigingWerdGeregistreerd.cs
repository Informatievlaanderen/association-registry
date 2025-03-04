namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Framework;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Constants;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formats;
using Framework;
using JsonLdContext;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Doelgroep = AssociationRegistry.Public.Schema.Detail.Doelgroep;
using VerenigingStatus = AssociationRegistry.Public.Schema.Constants.VerenigingStatus;

[UnitTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    [Fact]
    public void Then_it_creates_a_new_vereniging()
    {
        var fixture = new Fixture().CustomizePublicApi();

        var feitelijkeVerenigingWerdGeregistreerd =
            new TestEvent<FeitelijkeVerenigingWerdGeregistreerd>(fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>());

        feitelijkeVerenigingWerdGeregistreerd.StreamKey = feitelijkeVerenigingWerdGeregistreerd.Data.VCode;

        var doc = PubliekVerenigingDetailProjector.Create(feitelijkeVerenigingWerdGeregistreerd);

        doc.Should().BeEquivalentTo(
            new PubliekVerenigingDetailDocument
            {
                JsonLdMetadataType = JsonLdType.FeitelijkeVereniging.Type,
                VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                Verenigingstype = new PubliekVerenigingDetailDocument.VerenigingsType
                {
                    Code = Verenigingstype.FeitelijkeVereniging.Code,
                    Naam = Verenigingstype.FeitelijkeVereniging.Naam,
                },
                Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
                KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
                KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
                Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum,
                Doelgroep = new Doelgroep
                {
                    JsonLdMetadata =
                        new JsonLdMetadata(JsonLdType.Doelgroep.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
                                           JsonLdType.Doelgroep.Type),
                    Minimumleeftijd = feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep.Minimumleeftijd,
                    Maximumleeftijd = feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep.Maximumleeftijd,
                },
                DatumLaatsteAanpassing =
                    feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).FormatAsBelgianDate(),
                Status = VerenigingStatus.Actief,
                IsUitgeschrevenUitPubliekeDatastroom = feitelijkeVerenigingWerdGeregistreerd.Data.IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens.Select(
                    c => new PubliekVerenigingDetailDocument.Contactgegeven
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.Contactgegeven.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                                                                         c.ContactgegevenId.ToString()),
                            JsonLdType.Contactgegeven.Type),
                        ContactgegevenId = c.ContactgegevenId,
                        Contactgegeventype = c.Contactgegeventype.ToString(),
                        Waarde = c.Waarde,
                        Beschrijving = c.Beschrijving,
                        IsPrimair = c.IsPrimair,
                    }).ToArray(),
                Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties.Select(
                    loc => new PubliekVerenigingDetailDocument.Locatie
                    {
                        JsonLdMetadata =
                            new JsonLdMetadata(
                                JsonLdType.Locatie.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                                                                      loc.LocatieId.ToString()),
                                JsonLdType.Locatie.Type),
                        LocatieId = loc.LocatieId,
                        IsPrimair = loc.IsPrimair,
                        Naam = loc.Naam,
                        Locatietype = loc.Locatietype,
                        Adres = loc.Adres is null
                            ? null
                            : new PubliekVerenigingDetailDocument.Adres
                            {
                                JsonLdMetadata =
                                    new JsonLdMetadata(
                                        JsonLdType.Adres.CreateWithIdValues(doc.VCode, loc.LocatieId.ToString()),
                                        JsonLdType.Adres.Type),

                                Straatnaam = loc.Adres.Straatnaam,
                                Huisnummer = loc.Adres.Huisnummer,
                                Busnummer = loc.Adres.Busnummer,
                                Postcode = loc.Adres.Postcode,
                                Gemeente = loc.Adres.Gemeente,
                                Land = loc.Adres.Land,
                            },
                        Adresvoorstelling = loc.Adres.ToAdresString(),
                        AdresId = loc.AdresId is null
                            ? null
                            : new PubliekVerenigingDetailDocument.AdresId
                            {
                                Broncode = loc.AdresId?.Broncode,
                                Bronwaarde = loc.AdresId?.Bronwaarde,
                            },
                        VerwijstNaar = loc.AdresId is null
                            ? null
                            : new PubliekVerenigingDetailDocument.Locatie.AdresVerwijzing
                            {
                                JsonLdMetadata = new JsonLdMetadata
                                {
                                    Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(loc.AdresId.Bronwaarde.Split('/').Last()),
                                    Type = JsonLdType.AdresVerwijzing.Type,
                                },
                            },
                    }).ToArray(),
                HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                    arg => new PubliekVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.Hoofdactiviteit.CreateWithIdValues(arg.Code),
                            JsonLdType.Hoofdactiviteit.Type),
                        Code = arg.Code,
                        Naam = arg.Naam,
                    }).ToArray(),
                Werkingsgebieden = [],
                Sleutels = new[]
                {
                    new PubliekVerenigingDetailDocument.Sleutel
                    {
                        JsonLdMetadata = new JsonLdMetadata(
                            JsonLdType.Sleutel.CreateWithIdValues(feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                                                                  Sleutelbron.VR.Waarde),
                            JsonLdType.Sleutel.Type),
                        Bron = Sleutelbron.VR.Waarde,
                        GestructureerdeIdentificator = new PubliekVerenigingDetailDocument.GestructureerdeIdentificator
                        {
                            JsonLdMetadata = new JsonLdMetadata(
                                JsonLdType.GestructureerdeSleutel.CreateWithIdValues(
                                    feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                                    Sleutelbron.VR.Waarde),
                                JsonLdType.GestructureerdeSleutel.Type),
                            Nummer = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                        },
                        Waarde = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
                        CodeerSysteem = CodeerSysteem.VR,
                    },
                },
            });
    }
}
