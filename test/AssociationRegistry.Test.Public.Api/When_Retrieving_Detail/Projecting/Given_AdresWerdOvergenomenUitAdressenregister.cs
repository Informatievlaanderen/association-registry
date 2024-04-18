namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using Events;
using JsonLdContext;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using FluentAssertions;
using Formatters;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_AdresWerdOvergenomenUitAdressenregister
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();
        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        var adresWerdOvergenomen = new TestEvent<AdresWerdOvergenomenUitAdressenregister>(
            new AdresWerdOvergenomenUitAdressenregister(
                doc.VCode,
                locatieWerdToegevoegd.Data.Locatie.LocatieId,
                fixture.Create<AdresMatchUitGrar>(),
                fixture.CreateMany<AdresMatchUitGrar>().ToArray()));

        PubliekVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        PubliekVerenigingDetailProjector.Apply(adresWerdOvergenomen, doc);

        var locatie = doc.Locaties.Should().ContainSingle(locatie => locatie.LocatieId == locatieWerdToegevoegd.Data.Locatie.LocatieId)
                         .Subject;

        locatie.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.Locatie
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
            Adres = new PubliekVerenigingDetailDocument.Adres
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.Adres.CreateWithIdValues(doc.VCode, locatieWerdToegevoegd.Data.Locatie.LocatieId.ToString()),
                        Type = JsonLdType.Adres.Type,
                    },
                    Straatnaam = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.Adres.Straatnaam,
                    Huisnummer = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.Adres.Huisnummer,
                    Busnummer = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.Adres.Busnummer,
                    Postcode = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.Adres.Postcode,
                    Gemeente = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.Adres.Gemeente,
                    Land = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.Adres.Land,
                },
            Adresvoorstelling = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.Adres.ToAdresString(),
            AdresId = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.AdresId is null
                ? null
                : new PubliekVerenigingDetailDocument.AdresId
                {
                    Broncode = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.AdresId?.Broncode,
                    Bronwaarde = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.AdresId?.Bronwaarde,
                },
            VerwijstNaar = adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.AdresId is null
                ? null
                : new PubliekVerenigingDetailDocument.Locatie.AdresVerwijzing
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(adresWerdOvergenomen.Data.OvergenomenAdresUitGrar.AdresId?.Bronwaarde.Split('/').Last()),
                        Type = JsonLdType.AdresVerwijzing.Type,
                    },
                },
        });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}