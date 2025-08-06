namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VerenigingsNaam;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Empty_String
{
    [Fact]
    public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
    {
        var ctor = () => VerenigingsNaam.Create(string.Empty);
        ctor.Should().Throw<VerenigingsnaamIsLeeg>();
    }
}
