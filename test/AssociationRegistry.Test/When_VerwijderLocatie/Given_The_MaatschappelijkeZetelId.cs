namespace AssociationRegistry.Test.When_VerwijderLocatie;

using Events;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_The_MaatschappelijkeZetelId
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();
        var locatie = fixture.Create<Locatie>() with
        {
            LocatieId = 1,
            Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo,
            AdresId = null,
        };

        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(new VerenigingState().Apply(MaatschappelijkeZetelWerdOvergenomenUitKbo.With(locatie)));

        var wijzigLocatie = () => vereniging.VerwijderLocatie(1);

        wijzigLocatie.Should().Throw<MaatschappelijkeZetelCanNotBeRemoved>();
    }
}
