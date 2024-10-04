namespace AssociationRegistry.Test.When_Creating_A_WerkingsgebiedenLijst;

using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Werkingsgebieden
{
    [Fact]
    public void Then_It_Returns_A_Filled_WerkingsgebiedenLijst()
    {
        var fixture = new Fixture();

        var werkingsgebieden = Werkingsgebied.All
                                                               .OrderBy(_ => fixture.Create<int>())
                                                               .Take(2)
                                                               .ToArray();

        var werkingsgebiedenLijst = Werkingsgebieden.FromArray(werkingsgebieden);

        werkingsgebiedenLijst.Should().BeEquivalentTo(werkingsgebieden);
    }
}
