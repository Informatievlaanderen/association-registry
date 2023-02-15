namespace AssociationRegistry.Test.When_Creating_A_HoofdactiviteitenLijst;

using Activiteiten;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_A_List_Of_Hoofdactiviteiten
{
    [Fact]
    public void Then_It_Returns_A_Filled_HoofdactiviteitenLijst()
    {
        var fixture = new Fixture();
        var listOfHoofdactiviteiten = Hoofdactiviteit.All()
            .OrderBy(_ => fixture.Create<int>())
            .Take(2)
            .ToList();

        var hoofdactiviteitenLijst = HoofdactiviteitenLijst.Create(listOfHoofdactiviteiten);

        hoofdactiviteitenLijst.Should().BeEquivalentTo(listOfHoofdactiviteiten);
    }
}
