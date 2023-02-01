namespace AssociationRegistry.Test.When_Creating_A_VertegenwoordigersLijst;

using FluentAssertions;
using Vertegenwoordigers;
using Xunit;

public class Given_An_Empty_List
{
    [Fact]
    public void Then_It_Returns_An_Empty_VertegenwoordersLijst()
    {
        var list = Array.Empty<Vertegenwoordiger>();
        var vertegenwoordigersLijst = VertegenwoordigersLijst.Create(list);
        vertegenwoordigersLijst.Should().BeEmpty();
    }
}
