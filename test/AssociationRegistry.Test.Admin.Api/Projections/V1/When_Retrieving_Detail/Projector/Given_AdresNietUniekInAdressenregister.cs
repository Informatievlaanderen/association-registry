namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

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
public class Given_AdresNietUniekInAdressenregister
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();

        var adresWerdNietGevondenInAdressenregister = new TestEvent<AdresNietUniekInAdressenregister>(
            fixture.Create<AdresNietUniekInAdressenregister>() with
            {
                LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
            });

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        BeheerVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        BeheerVerenigingDetailProjector.Apply(adresWerdNietGevondenInAdressenregister, doc);

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
            IsPrimair = locatieWerdToegevoegd.Data.Locatie.IsPrimair,
            Naam = locatieWerdToegevoegd.Data.Locatie.Naam,
            Locatietype = locatieWerdToegevoegd.Data.Locatie.Locatietype,
            Adres = new Adres
            {
                JsonLdMetadata = new JsonLdMetadata
                {
                    Id = JsonLdType.Adres.CreateWithIdValues(doc.VCode, locatieWerdToegevoegd.Data.Locatie.LocatieId.ToString()),
                    Type = JsonLdType.Adres.Type,
                },
                Straatnaam = locatieWerdToegevoegd.Data.Locatie.Adres!.Straatnaam,
                Huisnummer = locatieWerdToegevoegd.Data.Locatie.Adres.Huisnummer,
                Busnummer = locatieWerdToegevoegd.Data.Locatie.Adres.Busnummer,
                Postcode = locatieWerdToegevoegd.Data.Locatie.Adres.Postcode,
                Gemeente = locatieWerdToegevoegd.Data.Locatie.Adres.Gemeente,
                Land = locatieWerdToegevoegd.Data.Locatie.Adres.Land,
            },
            Adresvoorstelling = locatieWerdToegevoegd.Data.Locatie.Adres.ToAdresString(),
            AdresId = null,
            VerwijstNaar = null,
            Bron = locatieWerdToegevoegd.Data.Bron,
        });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
