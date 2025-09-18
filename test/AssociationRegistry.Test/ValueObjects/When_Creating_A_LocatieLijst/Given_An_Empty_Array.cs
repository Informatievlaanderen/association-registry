namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_LocatieLijst;

using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_An_Empty_Array
{
    [Fact]
    public void Then_It_Returns_Empty()
    {
        var locaties = Locaties.Empty;
        locaties.Should().BeEmpty();
    }
}
