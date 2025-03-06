namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formats;
using Framework;
using JsonLdContext;
using System.Linq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_LocatieWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var locatieWerdGewijzigd = fixture.Create<TestEvent<LocatieWerdGewijzigd>>();

        var locatie = fixture.Create<PubliekVerenigingDetailDocument.Locatie>() with
        {
            LocatieId = locatieWerdGewijzigd.Data.Locatie.LocatieId,
        };

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(locatie).ToArray();

        PubliekVerenigingDetailProjector.Apply(locatieWerdGewijzigd, doc);

        doc.Locaties.Should().HaveCount(4);

        doc.Locaties.Should().ContainEquivalentOf(
            new PubliekVerenigingDetailDocument.Locatie
            {
                JsonLdMetadata =
                    new JsonLdMetadata(
                        JsonLdType.Locatie.CreateWithIdValues(doc.VCode, locatieWerdGewijzigd.Data.Locatie.LocatieId.ToString()),
                        JsonLdType.Locatie.Type),
                LocatieId = locatieWerdGewijzigd.Data.Locatie.LocatieId,
                IsPrimair = locatieWerdGewijzigd.Data.Locatie.IsPrimair,
                Naam = locatieWerdGewijzigd.Data.Locatie.Naam,
                Locatietype =
                    locatieWerdGewijzigd.Data.Locatie.Locatietype,
                Adres = locatieWerdGewijzigd.Data.Locatie.Adres is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Adres
                    {
                        JsonLdMetadata =
                            new JsonLdMetadata(
                                JsonLdType.Adres.CreateWithIdValues(doc.VCode, locatieWerdGewijzigd.Data.Locatie.LocatieId.ToString()),
                                JsonLdType.Adres.Type),
                        Straatnaam = locatieWerdGewijzigd.Data.Locatie.Adres.Straatnaam,
                        Huisnummer = locatieWerdGewijzigd.Data.Locatie.Adres.Huisnummer,
                        Busnummer = locatieWerdGewijzigd.Data.Locatie.Adres.Busnummer,
                        Postcode = locatieWerdGewijzigd.Data.Locatie.Adres.Postcode,
                        Gemeente = locatieWerdGewijzigd.Data.Locatie.Adres.Gemeente,
                        Land = locatieWerdGewijzigd.Data.Locatie.Adres.Land,
                    },
                Adresvoorstelling = locatieWerdGewijzigd.Data.Locatie.Adres.ToAdresString(),
                AdresId = locatieWerdGewijzigd.Data.Locatie.AdresId is null
                    ? null
                    : new PubliekVerenigingDetailDocument.AdresId
                    {
                        Broncode = locatieWerdGewijzigd.Data.Locatie.AdresId?.Broncode,
                        Bronwaarde = locatieWerdGewijzigd.Data.Locatie.AdresId?.Bronwaarde,
                    },
                VerwijstNaar = locatieWerdGewijzigd.Data.Locatie.AdresId is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Locatie.AdresVerwijzing
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                                locatieWerdGewijzigd.Data.Locatie.AdresId.Bronwaarde.Split('/').Last()),
                            Type = JsonLdType.AdresVerwijzing.Type,
                        },
                    },
            });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
