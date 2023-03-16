namespace AssociationRegistry.Test.When_Creating_A_HoofdactiviteitenLijst;

using AutoFixture;
using FluentAssertions;
using Hoofdactiviteiten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Hoofdactiviteiten
{
    [Fact]
    public void Then_It_Returns_A_Filled_HoofdactiviteitenLijst()
    {
        var fixture = new Fixture();
        var listOfHoofdactiviteiten = HoofdactiviteitVerenigingsloket.All()
            .OrderBy(_ => fixture.Create<int>())
            .Take(2)
            .ToList();

        var hoofdactiviteitenLijst = HoofdactiviteitenVerenigingsloketLijst.Create(listOfHoofdactiviteiten);

        hoofdactiviteitenLijst.Should().BeEquivalentTo(listOfHoofdactiviteiten);
    }
}
