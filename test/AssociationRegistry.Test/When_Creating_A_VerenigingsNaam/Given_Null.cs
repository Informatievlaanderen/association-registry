namespace AssociationRegistry.Test.When_Creating_A_VerenigingsNaam;

using FluentAssertions;
using VerenigingsNamen;
using VerenigingsNamen.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
    {
        var ctor = () => new VerenigingsNaam(null!);
        ctor.Should().Throw<EmptyVerenigingsNaam>();
    }
}
