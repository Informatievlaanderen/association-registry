namespace AssociationRegistry.Test.When_Creating_An_Insz;

using FluentAssertions;
using INSZ;
using INSZ.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_A_String_With_Non_Numeric_Characters
{
    [Theory]
    [InlineData("AABBCCDDDEE")]
    [InlineData("12AB34CDE56")]
    [InlineData("%$&*)(*&⁽)@")]
    public void Then_it_throws_an_InvalidInszCharsException(string insz)
    {
        var factory = () => Insz.Create(insz);
        factory.Should().Throw<InvalidInszChars>();
    }
}
