namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VerenigingsNaam;

using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Empty_String
{
    [Fact]
    public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
    {
        var ctor = () => VerenigingsNaam.Create(string.Empty);
        ctor.Should().Throw<VerenigingsnaamIsLeeg>();
    }
}
