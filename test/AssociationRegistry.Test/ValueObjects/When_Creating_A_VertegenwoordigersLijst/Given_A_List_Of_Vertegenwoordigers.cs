namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VertegenwoordigersLijst;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using System.Linq;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_List_Of_Vertegenwoordigers
{
    [Fact]
    public void Then_It_Returns_A_Filled_VertegenwoordigersLijst()
    {
        var fixture = new Fixture().CustomizeDomain();
        var listOfVertegenwoordigers = fixture.CreateMany<Vertegenwoordiger>().ToArray();

        var vertegenwoordigersLijst = Vertegenwoordigers.Empty.VoegToe(listOfVertegenwoordigers);

        vertegenwoordigersLijst.Should()
                               .BeEquivalentTo(
                                    listOfVertegenwoordigers,
                                    config: options => options.Excluding(vertegenwoordiger => vertegenwoordiger.VertegenwoordigerId));
    }
}
