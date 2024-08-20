namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_LocatieWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();

        var locatieWerdGewijzigd = new TestEvent<LocatieWerdGewijzigd>(
            new LocatieWerdGewijzigd(
                Locatie: fixture.Create<Registratiedata.Locatie>() with
                {
                    LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
                }));

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        BeheerVerenigingDetailProjector.Apply(locatieWerdGewijzigd, doc);

        doc.Locaties.Should().HaveCount(4);

        var locatie = doc.Locaties.Should().ContainSingle(locatie => locatie.LocatieId == locatieWerdToegevoegd.Data.Locatie.LocatieId)
                         .Subject;

        locatie.Should().BeEquivalentTo(new Locatie
        {
            JsonLdMetadata = new JsonLdMetadata
            {
                Id = JsonLdType.Locatie.CreateWithIdValues(doc.VCode, locatieWerdToegevoegd.Data.Locatie.LocatieId.ToString()),
                Type = JsonLdType.Locatie.Type,
            },
            LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
            IsPrimair = locatieWerdGewijzigd.Data.Locatie.IsPrimair,
            Naam = locatieWerdGewijzigd.Data.Locatie.Naam,
            Locatietype = locatieWerdGewijzigd.Data.Locatie.Locatietype,
            Adres = locatieWerdGewijzigd.Data.Locatie.Adres is null
                ? null
                : new Adres
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Adres.CreateWithIdValues(doc.VCode, locatieWerdToegevoegd.Data.Locatie.LocatieId.ToString()),
                        Type = JsonLdType.Adres.Type,
                    },
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
                : new AdresId
                {
                    Broncode = locatieWerdGewijzigd.Data.Locatie.AdresId?.Broncode,
                    Bronwaarde = locatieWerdGewijzigd.Data.Locatie.AdresId?.Bronwaarde,
                },
            VerwijstNaar = locatieWerdGewijzigd.Data.Locatie.AdresId is null
                ? null
                : new AdresVerwijzing
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(locatieWerdGewijzigd.Data.Locatie.AdresId?.Bronwaarde.Split('/').Last()),
                        Type = JsonLdType.AdresVerwijzing.Type,
                    },
                },
            Bron = locatieWerdToegevoegd.Data.Bron,
        });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
