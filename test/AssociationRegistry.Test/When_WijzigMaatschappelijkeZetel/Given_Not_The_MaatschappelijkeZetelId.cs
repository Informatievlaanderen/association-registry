namespace AssociationRegistry.Test.WhenWijzigMaatschappelijkeZetel;

using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
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
