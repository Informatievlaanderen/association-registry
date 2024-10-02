namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail.Projector;

using AssociationRegistry.Admin.ProjectionHost.Projections.Detail;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework;
using AutoFixture;
using FluentAssertions;
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

        var loc = doc.Locaties.Should().ContainSingle(l => l.LocatieId == locatie.LocatieId).Subject;

        loc.Should().BeEquivalentTo(
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
                VerwijstNaar = locatie.VerwijstNaar,
                Bron = locatie.Bron,
            }
        );

        doc.Locaties.Should().BeInAscendingOrder(l => l.LocatieId);
    }
}
