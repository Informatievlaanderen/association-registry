namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formats;
using Framework;
using JsonLdContext;
using Xunit;

public class Given_LocatieWerdToegevoegd
{
    [Fact]
    public void Then_it_adds_a_locatie()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var locatieWerdToegevoegd = new TestEvent<LocatieWerdToegevoegd>(fixture.Create<LocatieWerdToegevoegd>());

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);

        doc.Locaties.Should().HaveCount(4);

        doc.Locaties.Should().ContainEquivalentOf(
            new PubliekVerenigingDetailDocument.Types.Locatie
            {
                JsonLdMetadata =
                    new JsonLdMetadata(
                        JsonLdType.Locatie.CreateWithIdValues(doc.VCode, locatieWerdToegevoegd.Data.Locatie.LocatieId.ToString()),
                        JsonLdType.Locatie.Type),
                LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
                IsPrimair = locatieWerdToegevoegd.Data.Locatie.IsPrimair,
                Naam = locatieWerdToegevoegd.Data.Locatie.Naam,
                Locatietype =
                    locatieWerdToegevoegd.Data.Locatie.Locatietype,
                Adres = locatieWerdToegevoegd.Data.Locatie.Adres is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Types.Adres
                    {
                        JsonLdMetadata =
                            new JsonLdMetadata(
                                JsonLdType.Adres.CreateWithIdValues(doc.VCode, locatieWerdToegevoegd.Data.Locatie.LocatieId.ToString()),
                                JsonLdType.Adres.Type),
                        Straatnaam = locatieWerdToegevoegd.Data.Locatie.Adres.Straatnaam,
                        Huisnummer = locatieWerdToegevoegd.Data.Locatie.Adres.Huisnummer,
                        Busnummer = locatieWerdToegevoegd.Data.Locatie.Adres.Busnummer,
                        Postcode = locatieWerdToegevoegd.Data.Locatie.Adres.Postcode,
                        Gemeente = locatieWerdToegevoegd.Data.Locatie.Adres.Gemeente,
                        Land = locatieWerdToegevoegd.Data.Locatie.Adres.Land,
                    },
                Adresvoorstelling = locatieWerdToegevoegd.Data.Locatie.Adres.ToAdresString(),
                AdresId = locatieWerdToegevoegd.Data.Locatie.AdresId is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Types.AdresId
                    {
                        Broncode = locatieWerdToegevoegd.Data.Locatie.AdresId?.Broncode,
                        Bronwaarde = locatieWerdToegevoegd.Data.Locatie.AdresId?.Bronwaarde,
                    },
                VerwijstNaar = locatieWerdToegevoegd.Data.Locatie.AdresId is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Types.Locatie.AdresVerwijzing
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                                locatieWerdToegevoegd.Data.Locatie.AdresId.Bronwaarde.Split('/').Last()),
                            Type = JsonLdType.AdresVerwijzing.Type,
                        },
                    },
            });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
