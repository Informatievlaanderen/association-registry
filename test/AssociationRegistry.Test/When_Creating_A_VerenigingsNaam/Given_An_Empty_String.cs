namespace AssociationRegistry.Test.When_Creating_A_VerenigingsNaam;

using FluentAssertions;
using VerenigingsNamen;
using VerenigingsNamen.Exceptions;
using Xunit;

public class Given_An_Empty_String
{
    [Fact]
    public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
    {
        var ctor = () => new VerenigingsNaam(String.Empty);
        ctor.Should().Throw<EmptyVerenigingsNaam>();
    }
}
