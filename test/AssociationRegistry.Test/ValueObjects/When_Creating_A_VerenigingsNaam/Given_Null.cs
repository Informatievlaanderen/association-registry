namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VerenigingsNaam;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_Null
{
    [Fact]
    public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
    {
        var ctor = () => VerenigingsNaam.Create(null!);
        ctor.Should().Throw<VerenigingsnaamIsLeeg>();
    }
}
