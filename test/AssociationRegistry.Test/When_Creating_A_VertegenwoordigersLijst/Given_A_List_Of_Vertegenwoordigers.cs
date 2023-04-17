namespace AssociationRegistry.Test.When_Creating_A_VertegenwoordigersLijst;

using AutoFixture;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Vertegenwoordigers
{
    [Fact]
    public void Then_It_Returns_A_Filled_VertegenwoordigersLijst()
    {
        var fixture = new Fixture().CustomizeAll();
        var listOfVertegenwoordigers = fixture.CreateMany<Vertegenwoordiger>().ToArray();

        var vertegenwoordigersLijst = Vertegenwoordigers.FromArray(listOfVertegenwoordigers);

        vertegenwoordigersLijst.Should().BeEquivalentTo(listOfVertegenwoordigers);
    }
}
