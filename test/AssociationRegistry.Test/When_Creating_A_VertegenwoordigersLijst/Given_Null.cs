namespace AssociationRegistry.Test.When_Creating_A_VertegenwoordigersLijst;

using FluentAssertions;
using Vertegenwoordigers;
using Xunit;

public class Given_Null
{
    [Fact]
    public void Then_It_Returns_An_Empty_VertegenwoordigersLijst()
    {
        VertegenwoordigersLijst.Create(null).Should().BeEquivalentTo(VertegenwoordigersLijst.Empty);
    }
}
