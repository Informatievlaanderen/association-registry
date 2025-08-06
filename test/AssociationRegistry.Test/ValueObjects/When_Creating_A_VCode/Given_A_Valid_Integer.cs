namespace AssociationRegistry.Test.ValueObjects.When_Creating_A_VCode;

using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Valid_Integer
{
    [Theory]
    [InlineData(1001, "V0001001")]
    [InlineData(1002, "V0001002")]
    [InlineData(1234, "V0001234")]
    [InlineData(9876543, "V9876543")]
    [InlineData(9999999, "V9999999")]
    public void Then_it_returns_a_new_vcode_formatted_as_V0000000(int intCode, string expectedVCode)
    {
        var vCode = VCode.Create(intCode);

        vCode.Value.Should().Be(expectedVCode);
    }
}
