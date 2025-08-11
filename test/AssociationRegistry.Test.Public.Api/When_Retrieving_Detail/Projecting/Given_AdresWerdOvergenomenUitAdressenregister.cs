namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Contracts.JsonLdContext;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using FluentAssertions;
using Formats;
using Framework;
using Vereniging;
using Xunit;

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
                fixture.Create<Registratiedata.AdresId>(),
                fixture.Create<Registratiedata.AdresUitAdressenregister>()
            ));

        PubliekVerenigingDetailProjector.Apply(locatieWerdToegevoegd, doc);
        PubliekVerenigingDetailProjector.Apply(adresWerdOvergenomen, doc);

        var locatie = doc.Locaties.Should().ContainSingle(locatie => locatie.LocatieId == locatieWerdToegevoegd.Data.Locatie.LocatieId)
                         .Subject;

        locatie.Should().BeEquivalentTo(new PubliekVerenigingDetailDocument.Types.Locatie
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
            Adres = new PubliekVerenigingDetailDocument.Types.Adres
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
                Land = Adres.België,
            },
            Adresvoorstelling = adresWerdOvergenomen.Data.Adres.ToAdresString(),
            AdresId = adresWerdOvergenomen.Data.AdresId is null
                ? null
                : new PubliekVerenigingDetailDocument.Types.AdresId
                {
                    Broncode = adresWerdOvergenomen.Data.AdresId?.Broncode,
                    Bronwaarde = adresWerdOvergenomen.Data.AdresId?.Bronwaarde,
                },
            VerwijstNaar = adresWerdOvergenomen.Data.AdresId is null
                ? null
                : new PubliekVerenigingDetailDocument.Types.Locatie.AdresVerwijzing
                {
                    JsonLdMetadata = new JsonLdMetadata
                    {
                        Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                            adresWerdOvergenomen.Data.AdresId?.Bronwaarde.Split('/').Last()),
                        Type = JsonLdType.AdresVerwijzing.Type,
                    },
                },
        });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
