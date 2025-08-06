namespace AssociationRegistry.Test.Locaties.When_VerwijderLocatie;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using Events.Factories;
using FluentAssertions;
using Xunit;

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

        vereniging.Hydrate(new VerenigingState()
                          .Apply(fixture.Create<LocatieWerdToegevoegd>() with
                           {
                               Locatie = fixture.Create<Registratiedata.Locatie>() with
                               {
                                   LocatieId = 2,
                               },
                           })
                          .Apply(EventFactory.MaatschappelijkeZetelWerdOvergenomenUitKbo(locatie)));

        var wijzigLocatie = () => vereniging.VerwijderLocatie(1);

        wijzigLocatie.Should().Throw<MaatschappelijkeZetelKanNietVerwijderdWorden>();
    }
}
