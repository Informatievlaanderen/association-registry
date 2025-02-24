namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_AdresWerdOntkoppeldVanAdressenregister
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();

        BeheerVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);

        var adresWerdOvergenomenUitAdressenregister = new TestEvent<AdresWerdOvergenomenUitAdressenregister>(
            fixture.Create<AdresWerdOvergenomenUitAdressenregister>() with
            {
                AdresId = locatieWerdToegevoegd.Data.Locatie.AdresId,
                Adres = new Registratiedata.AdresUitAdressenregister(
                    locatieWerdToegevoegd.Data.Locatie.Adres.Straatnaam,
                    locatieWerdToegevoegd.Data.Locatie.Adres.Huisnummer,
                    locatieWerdToegevoegd.Data.Locatie.Adres.Busnummer,
                    locatieWerdToegevoegd.Data.Locatie.Adres.Postcode,
                    locatieWerdToegevoegd.Data.Locatie.Adres.Gemeente
                ),
                LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
            });

        BeheerVerenigingDetailProjector.Apply(adresWerdOvergenomenUitAdressenregister, doc);

        var adresWerdOntkoppeldVanAdressenregister = new TestEvent<AdresWerdOntkoppeldVanAdressenregister>(
            fixture.Create<AdresWerdOntkoppeldVanAdressenregister>() with
            {
                LocatieId = locatieWerdToegevoegd.Data.Locatie.LocatieId,
            });

        BeheerVerenigingDetailProjector.Apply(adresWerdOntkoppeldVanAdressenregister, doc);

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
