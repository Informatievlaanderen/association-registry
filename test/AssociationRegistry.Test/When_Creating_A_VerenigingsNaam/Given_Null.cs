namespace AssociationRegistry.Test.When_Creating_A_VerenigingsNaam;

using FluentAssertions;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_Null
{
    [Fact]
    public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
    {
        var ctor = () => VerenigingsNaam.Create(null!);
        ctor.Should().Throw<VerenigingsnaamIsLeeg>();
    }
}
