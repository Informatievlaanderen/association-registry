namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Formatters;
using Framework;
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
                LocatieId = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.LocatieId,
                IsPrimair = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.IsPrimair,
                Naam = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Naam,
                Locatietype = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Locatietype,
                Adres = maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie.Adres is null
                    ? null
                    : new PubliekVerenigingDetailDocument.Adres
                    {
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
            });
        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
        doc.DatumLaatsteAanpassing.Should().Be(maatschappelijkeZetelWerdOvergenomenUitKbo.Tijdstip.ToBelgianDate());
    }
}
