namespace AssociationRegistry.Test.Locaties.When_WijzigLocatie;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using DecentraalBeheer.Vereniging.Exceptions;
using Events.Factories;
using FluentAssertions;
using Xunit;

public class Given_From_MaatschappelijkeZetelId
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
        vereniging.Hydrate(new VerenigingState().Apply(EventFactory.MaatschappelijkeZetelWerdOvergenomenUitKbo(locatie)));

        var wijzigLocatie = () => vereniging.WijzigLocatie(
            locatieId: 1,
            fixture.Create<string>(),
            Locatietype.Activiteiten,
            isPrimair: true,
            fixture.Create<AdresId>(),
            fixture.Create<Adres>());

        wijzigLocatie.Should().Throw<MaatschappelijkeZetelKanNietGewijzigdWorden>();
    }
}

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
