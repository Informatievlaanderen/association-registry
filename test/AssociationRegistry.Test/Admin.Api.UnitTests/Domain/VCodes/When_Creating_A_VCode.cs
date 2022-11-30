namespace AssociationRegistry.Test.Admin.Api.UnitTests.Domain.VCodes;

using AssociationRegistry.VCodes;
using AssociationRegistry.VCodes.Exceptions;
using FluentAssertions;
using Xunit;

public class When_Creating_A_VCode
{
    public class Given_A_Valid_Integer
    {
        [Theory]
        [InlineData(1001, "V1001")]
        [InlineData(1002, "V1002")]
        [InlineData(1234, "V1234")]
        [InlineData(9876543, "V9876543")]
        [InlineData(9999999, "V9999999")]
        [InlineData(1_234_567_890, "V1234567890")]
        public void Then_it_returns_a_new_vcode_formatted_as_V0000(int intCode, string expectedVCode)
        {
            var vCode = VCode.Create(intCode);

            vCode.Value.Should().Be(expectedVCode);
        }
    }

    public class Given_An_Integer_Smaller_Than_A_Thousand_And_One
    {
        [Theory]
        [InlineData(1000)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1000)]
        public void Then_it_throws_an_InvalidVCodeException(int intCode)
        {
            var ctor = () => VCode.Create(intCode);
            ctor.Should().Throw<OutOfRangeVCode>();
        }
    }

    public class Given_A_Valid_String_With_Capital_V
    {
        [Theory]
        [InlineData("V1001")]
        [InlineData("V1002")]
        [InlineData("V845685")]
        [InlineData("V999999")]
        public void Then_it_returns_a_new_vcode_formatted_as_V0000(string strCode)
        {
            var vCode = VCode.Create(strCode);

            vCode.Value.Should().Be(strCode);
        }
    }

    public class Given_A_Valid_String_With_Lower_Case_V
    {
        [Theory]
        [InlineData("v1001", "V1001")]
        [InlineData("v1002", "V1002")]
        [InlineData("v458565", "V458565")]
        [InlineData("v999999", "V999999")]
        public void Then_it_returns_a_new_vcode_formatted_as_V0000(string strCode, string expectedVCode)
        {
            var vCode = VCode.Create(strCode);

            vCode.Value.Should().Be(expectedVCode);
        }
    }

    public class Given_A_String_Not_Starting_With_V
    {
        [Theory]
        [InlineData("1234567")]
        [InlineData("A123456")]
        [InlineData("ABCDEFG")]
        public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
        {
            var ctor = () => VCode.Create(strCode);
            ctor.Should().Throw<InvalidVCodeFormat>();
        }
    }

    public class Given_A_String_Not_Ending_With_Numbers
    {
        [Theory]
        [InlineData("VBCDEFG")]
        [InlineData("V12345A")]
        public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
        {
            var ctor = () => VCode.Create(strCode);
            ctor.Should().Throw<InvalidVCodeFormat>();
        }
    }
}
