namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_Werkingsgebieden;

using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_Werkingsgebieden_With_NVT
{
    [Fact]
    public void Then_It_Throws_A_NietVanToepassingKanNietGecombineerdWordenException()
    {
        var fixture = new Fixture();

        var werkingsgebieden = Werkingsgebieden.NietVanToepassing
                                               .Union(
                                                    WellKnownWerkingsgebieden
                                                       .Provincies
                                                       .OrderBy(_ => fixture.Create<int>())
                                                       .Take(2)
                                                )
                                               .ToArray();

        var ctor = () => Werkingsgebieden.FromArray(werkingsgebieden);

        ctor.Should().Throw<WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden>();
    }
}
