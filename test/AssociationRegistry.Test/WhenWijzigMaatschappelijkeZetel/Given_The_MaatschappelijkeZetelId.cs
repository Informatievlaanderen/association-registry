namespace AssociationRegistry.Test.WhenWijzigMaatschappelijkeZetel;

using Events;
using Framework.Customizations;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_To_MaatschappelijkeZetelId
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var locatie = fixture.Create<Locatie>() with
        {
            LocatieId = 1,
        };

        var vereniging = new VerenigingOfAnyKind();
        vereniging.Hydrate(new VerenigingState().Apply(MaatschappelijkeZetelWerdOvergenomenUitKbo.With(locatie)));

        var wijzigLocatie = () => vereniging.WijzigLocatie(
            1,
            fixture.Create<string>(),
            Locatietype.MaatschappelijkeZetelVolgensKbo,
            fixture.Create<bool>(),
            fixture.Create<AdresId>(),
            fixture.Create<Adres>());

        wijzigLocatie.Should().Throw<MaatschappelijkeZetelIsNietToegestaan>();
    }
}
