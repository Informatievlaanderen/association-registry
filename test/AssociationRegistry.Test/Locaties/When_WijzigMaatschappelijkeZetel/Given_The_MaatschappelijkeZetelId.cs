namespace AssociationRegistry.Test.Locaties.When_WijzigMaatschappelijkeZetel;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using EventFactories;
using FluentAssertions;
using Xunit;

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
        vereniging.Hydrate(new VerenigingState().Apply(EventFactory.MaatschappelijkeZetelWerdOvergenomenUitKbo(locatie)));

        var wijzigLocatie = () => vereniging.WijzigLocatie(
            locatieId: 1,
            fixture.Create<string>(),
            Locatietype.MaatschappelijkeZetelVolgensKbo,
            fixture.Create<bool>(),
            fixture.Create<AdresId>(),
            fixture.Create<Adres>());

        wijzigLocatie.Should().Throw<MaatschappelijkeZetelIsNietToegestaan>();
    }
}
