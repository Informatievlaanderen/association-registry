namespace AssociationRegistry.Test.Locaties.When_WijzigMaatschappelijkeZetel;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Not_The_MaatschappelijkeZetelId
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var locatie = fixture.Create<Locatie>() with
        {
            LocatieId = 1,
            Locatietype = Locatietype.Correspondentie,
            AdresId = null,
        };

        var vereniging = new VerenigingMetRechtspersoonlijkheid();

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>())
                          .Apply(LocatieWerdToegevoegd.With(locatie)));

        var wijzigLocatie = () => vereniging.WijzigMaatschappelijkeZetel(
            locatieId: 1,
            fixture.Create<string>(),
            fixture.Create<bool>());

        wijzigLocatie.Should().Throw<ActieIsNietToegestaanVoorLocatieType>();
    }
}
