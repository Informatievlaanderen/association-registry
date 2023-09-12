namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using AssociationRegistry.Public.ProjectionHost.Projections.Detail;
using AssociationRegistry.Public.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_MaatschappelijkeZetelWerdGewijzigd
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizePublicApi();
        var maatschappelijkeZetelVolgensKboWerdGewijzigd = fixture.Create<TestEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>>();

        var locatie = fixture.Create<PubliekVerenigingDetailDocument.Locatie>() with
        {
            LocatieId = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId,
        };

        var doc = fixture.Create<PubliekVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(locatie).ToArray();

        PubliekVerenigingDetailProjector.Apply(maatschappelijkeZetelVolgensKboWerdGewijzigd, doc);

        doc.Locaties.Should().HaveCount(4);
        doc.Locaties.Should().ContainEquivalentOf(
            new PubliekVerenigingDetailDocument.Locatie
            {
                LocatieId = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId,
                IsPrimair = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.IsPrimair,
                Naam = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam,
                Locatietype = locatie.Locatietype,
                Adres = locatie.Adres,
                Adresvoorstelling = locatie.Adresvoorstelling,
                AdresId = locatie.AdresId,
            });
        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
        doc.DatumLaatsteAanpassing.Should().Be(maatschappelijkeZetelVolgensKboWerdGewijzigd.Tijdstip.ToBelgianDate());
    }
}
