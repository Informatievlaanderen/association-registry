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
public class Given_AdresWerdGewijzigdInAdressenregister
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var locatieWerdToegevoegd = fixture.Create<TestEvent<LocatieWerdToegevoegd>>();
        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        var adresWerdGewijzigdInAdressenregister = new TestEvent<AdresWerdGewijzigdInAdressenregister>(
            new AdresWerdGewijzigdInAdressenregister(
                doc.VCode,
                locatieWerdToegevoegd.Data.Locatie.LocatieId,
                fixture.Create<AdresDetailUitAdressenregister>(),
                "IdempotenceKey"));

        PubliekVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        PubliekVerenigingDetailProjector.Apply(adresWerdGewijzigdInAdressenregister, doc);

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
                    Straatnaam = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.Adres.Straatnaam,
                    Huisnummer = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.Adres.Huisnummer,
                    Busnummer = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.Adres.Busnummer,
                    Postcode = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.Adres.Postcode,
                    Gemeente = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.Adres.Gemeente,
                    Land = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.Adres.Land,
                },
            Adresvoorstelling = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.Adres.ToAdresString(),
            AdresId = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.AdresId is null
                ? null
                : new PubliekVerenigingDetailDocument.AdresId
                {
                    Broncode = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.AdresId?.Broncode,
                    Bronwaarde = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.AdresId?.Bronwaarde,
                },
            VerwijstNaar = adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.AdresId is null
                ? null
                : new PubliekVerenigingDetailDocument.Locatie.AdresVerwijzing
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(adresWerdGewijzigdInAdressenregister.Data.AdresDetailUitAdressenregister.AdresId?.Bronwaarde.Split('/').Last()),
                        Type = JsonLdType.AdresVerwijzing.Type,
                    },
                },
        });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
