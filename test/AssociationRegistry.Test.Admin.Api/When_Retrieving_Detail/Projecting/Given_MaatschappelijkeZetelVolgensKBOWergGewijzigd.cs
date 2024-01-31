namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail.Projecting;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AutoFixture;
using Events;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_MaatschappelijkeZetelVolgensKBOWergGewijzigd
{
    [Fact]
    public void Then_it_updates_a_locatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var maatschappelijkeZetelVolgensKboWerdGewijzigd = fixture.Create<TestEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd>>();

        var locatie = fixture.Create<Locatie>() with
        {
            LocatieId = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId,
        };

        var doc = fixture.Create<BeheerVerenigingDetailDocument>();
        doc.Locaties = doc.Locaties.Append(locatie).ToArray();

        BeheerVerenigingDetailProjector.Apply(maatschappelijkeZetelVolgensKboWerdGewijzigd, doc);

        doc.Locaties.Should().HaveCount(4);

        doc.Locaties.Should().ContainEquivalentOf(
            new Locatie
            {
                JsonLdMetadata = locatie.JsonLdMetadata,
                LocatieId = locatie.LocatieId,
                IsPrimair = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.IsPrimair,
                Naam = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam,
                Locatietype = locatie.Locatietype,
                Adres = locatie.Adres,
                Adresvoorstelling = locatie.Adresvoorstelling,
                AdresId = locatie.AdresId,
                Bron = locatie.Bron,
            });

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
