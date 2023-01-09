namespace AssociationRegistry.Test;

using VCodes;
using VCodes.Exceptions;
using FluentAssertions;
using Xunit;

public class When_Creating_A_VCode
{
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
        [InlineData("V0001001")]
        [InlineData("V0001002")]
        [InlineData("V0845685")]
        [InlineData("V9999999")]
        public void Then_it_returns_a_new_vcode_formatted_as_V0000000(string strCode)
        {
            var vCode = VCode.Create(strCode);

            vCode.Value.Should().Be(strCode);
        }
    }

    public class Given_A_Valid_String_With_Lower_Case_V
    {
        [Theory]
        [InlineData("v0001001", "V0001001")]
        [InlineData("v0001002", "V0001002")]
        [InlineData("v0458565", "V0458565")]
        [InlineData("v9999999", "V9999999")]
        public void Then_it_returns_a_new_vcode_formatted_as_V0000000(string strCode, string expectedVCode)
        {
            var vCode = VCode.Create(strCode);

            vCode.Value.Should().Be(expectedVCode);
        }
    }

    public class Given_A_String_Not_Starting_With_V
    {
        [Theory]
        [InlineData("12345678")]
        [InlineData("A1234567")]
        [InlineData("ABCDEFGH")]
        public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
        {
            var ctor = () => VCode.Create(strCode);
            ctor.Should().Throw<InvalidVCodeFormat>();
        }
    }

    public class Given_A_String_Not_Ending_With_Numbers
    {
        [Theory]
        [InlineData("VBCDEFGH")]
        [InlineData("V12345AB")]
        public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
        {
            var ctor = () => VCode.Create(strCode);
            ctor.Should().Throw<InvalidVCodeFormat>();
        }
    }

    public class Given_A_String_With_Less_Than_Eight_Characters
    {
        [Theory]
        [InlineData("V123456")]
        [InlineData("V1")]
        public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
        {
            var ctor = () => VCode.Create(strCode);
            ctor.Should().Throw<InvalidVCodeFormat>();
        }
    }
    public class Given_A_String_With_More_Than_Eight_Characters
    {
        [Theory]
        [InlineData("V12345678")]
        [InlineData("V12345678901234567890")]
        public void Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
        {
            var ctor = () => VCode.Create(strCode);
            ctor.Should().Throw<InvalidVCodeFormat>();
        }
    }
}
