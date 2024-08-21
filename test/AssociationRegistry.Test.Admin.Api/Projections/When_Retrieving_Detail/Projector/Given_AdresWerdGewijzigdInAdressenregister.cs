namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail.Projector;

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
public class Given_AdresWerdGewijzigdInAdressenregister
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();
        var doc = fixture.Create<BeheerVerenigingDetailDocument>();

        var adresWerdGewijzigdInAdressenregister = new TestEvent<AdresWerdGewijzigdInAdressenregister>(
            new AdresWerdGewijzigdInAdressenregister(
                doc.VCode,
                locatieWerdToegevoegd.Data.Locatie.LocatieId,
                fixture.Create<Registratiedata.AdresId>(),
                fixture.Create<Registratiedata.AdresUitAdressenregister>(),
                IdempotenceKey: "IdempotenceKey"));

        BeheerVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        BeheerVerenigingDetailProjector.Apply(adresWerdGewijzigdInAdressenregister, doc);

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
                Straatnaam = adresWerdGewijzigdInAdressenregister.Data.Adres.Straatnaam,
                Huisnummer = adresWerdGewijzigdInAdressenregister.Data.Adres.Huisnummer,
                Busnummer = adresWerdGewijzigdInAdressenregister.Data.Adres.Busnummer,
                Postcode = adresWerdGewijzigdInAdressenregister.Data.Adres.Postcode,
                Gemeente = adresWerdGewijzigdInAdressenregister.Data.Adres.Gemeente,
                Land = Vereniging.Adres.België,
            },
            Adresvoorstelling = adresWerdGewijzigdInAdressenregister.Data.Adres.ToAdresString(),
            AdresId = adresWerdGewijzigdInAdressenregister.Data.AdresId is null
                ? null
                : new AdresId
                {
                    Broncode = adresWerdGewijzigdInAdressenregister.Data.AdresId?.Broncode,
                    Bronwaarde = adresWerdGewijzigdInAdressenregister.Data.AdresId?.Bronwaarde,
                },
            VerwijstNaar = adresWerdGewijzigdInAdressenregister.Data.AdresId is null
                ? null
                : new AdresVerwijzing
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                            adresWerdGewijzigdInAdressenregister.Data.AdresId.Bronwaarde.Split('/').Last()),
                        Type = JsonLdType.AdresVerwijzing.Type,
                    },
                },
            Bron = locatieWerdToegevoegd.Data.Bron,
        });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
