namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formatters;
using Framework;
using JsonLdContext;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_MaatschappelijkeZetelWerdOvergenomenUitKbo
{
    [Fact]
    public void Then_it_adds_a_maatschappelijkeZetel()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var maatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<TestEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo>>();

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();

        PubliekVerenigingDetailProjector.Apply(maatschappelijkeZetelWerdOvergenomenUitKbo, doc);

        doc.Locaties.Should().HaveCount(4);

        doc.Locaties.Should().ContainEquivalentOf(
            new PubliekVerenigingDetailDocument.Locatie
            {
                JsonLdMetadata =
                    new JsonLdMetadata(
                        JsonLdType.Locatie.CreateWithIdValues(
                            doc.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId.ToString()),
                        JsonLdType.Locatie.Type),
                LocatieId = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId,
                IsPrimair = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.IsPrimair,
                Naam = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Naam,
                Locatietype =
                    maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Locatietype,
                Adres = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Adres
                    {
                        JsonLdMetadata =
                            new JsonLdMetadata(
                                JsonLdType.Adres.CreateWithIdValues(
                                    doc.VCode, maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId.ToString()),
                                JsonLdType.Adres.Type),
                        Straatnaam = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Straatnaam,
                        Huisnummer = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Huisnummer,
                        Busnummer = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Busnummer,
                        Postcode = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Postcode,
                        Gemeente = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Gemeente,
                        Land = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.Land,
                    },
                Adresvoorstelling = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres.ToAdresString(),
                AdresId = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId is null
                    ? null
                    : new PubliekVerenigingDetailDocument.AdresId
                    {
                        Broncode = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId?.Broncode,
                        Bronwaarde = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId?.Bronwaarde,
                    },
                VerwijstNaar = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Locatie.AdresVerwijzing
                    {
                        JsonLdMetadata = new JsonLdMetadata
                        {
                            Id = JsonLdType.AdresVerwijzing.CreateWithIdValues(
                                maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.AdresId.Bronwaarde.Split('/').Last()),
                            Type = JsonLdType.AdresVerwijzing.Type,
                        },
                    },
            });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
