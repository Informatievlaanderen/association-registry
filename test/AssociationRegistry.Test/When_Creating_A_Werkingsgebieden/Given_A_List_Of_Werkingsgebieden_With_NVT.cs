namespace AssociationRegistry.Test.When_Creating_A_Werkingsgebieden;

using AutoFixture;
using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Werkingsgebieden_With_NVT
{
    [Fact]
    public void Then_It_Throws_A_NietVanToepassingKanNietGecombineerdWordenException()
    {
        var fixture = new Fixture();

        var werkingsgebieden = Werkingsgebieden.NietVanToepassing
                                               .Union(
                                                    Werkingsgebied
                                                       .All
                                                       .OrderBy(_ => fixture.Create<int>())
                                                       .Take(2)
                                                )
                                               .ToArray();

        var ctor = () => Werkingsgebieden.FromArray(werkingsgebieden);

        ctor.Should().Throw<WerkingsgebiedNietVanToepassingKanNietGecombineerdWorden>();
    }
}
