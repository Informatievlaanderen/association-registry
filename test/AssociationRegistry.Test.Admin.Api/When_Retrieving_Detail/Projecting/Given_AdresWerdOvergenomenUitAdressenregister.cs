namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formats;
using Framework;
using JsonLdContext;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Schema.Detail.Adres;
using AdresId = AssociationRegistry.Admin.Schema.Detail.AdresId;
using Locatie = AssociationRegistry.Admin.Schema.Detail.Locatie;

[UnitTest]
public class Given_AdresWerdOvergenomenUitAdressenregister
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        var adresWerdOvergenomen = new TestEvent<AdresWerdOvergenomenUitAdressenregister>(
            new AdresWerdOvergenomenUitAdressenregister(
                doc.VCode,
                locatieWerdToegevoegd.Data.Locatie.LocatieId,
                fixture.Create<Registratiedata.AdresId>(),
                fixture.Create<Registratiedata.AdresUitAdressenregister>()));

        BeheerVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        BeheerVerenigingDetailProjector.Apply(adresWerdOvergenomen, doc);

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
                    Straatnaam = adresWerdOvergenomen.Data.Adres.Straatnaam,
                    Huisnummer = adresWerdOvergenomen.Data.Adres.Huisnummer,
                    Busnummer = adresWerdOvergenomen.Data.Adres.Busnummer,
                    Postcode = adresWerdOvergenomen.Data.Adres.Postcode,
                    Gemeente = adresWerdOvergenomen.Data.Adres.Gemeente,
                    Land = Vereniging.Adres.België,
                },
            Adresvoorstelling = adresWerdOvergenomen.Data.Adres.ToAdresString(),
            AdresId = adresWerdOvergenomen.Data.AdresId is null
                ? null
                : new AdresId
                {
                    Broncode = adresWerdOvergenomen.Data.AdresId?.Broncode,
                    Bronwaarde = adresWerdOvergenomen.Data.AdresId?.Bronwaarde,
                },
            VerwijstNaar = adresWerdOvergenomen.Data.AdresId is null
                ? null
                : new AdresVerwijzing
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(adresWerdOvergenomen.Data.AdresId?.Bronwaarde.Split('/').Last()),
                        Type = JsonLdType.AdresVerwijzing.Type,
                    },
                },
            Bron = locatieWerdToegevoegd.Data.Bron,
        });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
