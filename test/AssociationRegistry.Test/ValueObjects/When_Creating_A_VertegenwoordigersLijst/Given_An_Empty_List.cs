namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VertegenwoordigersLijst;

using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_List
{
    [Fact]
    public void Then_It_Returns_An_Empty_VertegenwoordersLijst()
    {
        var list = Array.Empty<Vertegenwoordiger>();
        var vertegenwoordigersLijst = Vertegenwoordigers.Empty.VoegToe(list);
        vertegenwoordigersLijst.Should().BeEmpty();
    }
}
