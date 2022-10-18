namespace AssociationRegistry.Test.Admin.Api.UnitTests.Domain.VCodes;

using AssociationRegistry.Admin.Api.Verenigingen.VCodes;
using AssociationRegistry.Admin.Api.Verenigingen.VCodes.Exceptions;
using FluentAssertions;
using Xunit;

public class When_Creating_A_VCode
{
    public class Given_A_Valid_Integer
    {
        [Theory]
        [InlineData(1, "V000001")]
        [InlineData(2, "V000002")]
        [InlineData(1234, "V001234")]
        [InlineData(987654, "V987654")]
        [InlineData(999999, "V999999")]
        public void Then_it_returns_a_new_vcode(int intCode, string expectedVCode)
        {
            var vCode = new VCode(intCode);

            vCode.Value.Should().Be(expectedVCode);
        }
    }

    public class Given_An_Invalid_Integer
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1000)]
        [InlineData(1_000_000)]
        [InlineData(123_456_789)]
        public void Then_it_throws_an_InvalidVCodeException(int intCode)
        {
            var ctor = () => new VCode(intCode);
            ctor.Should().Throw<OutOfRangeVCode>();
        }
    }

    public class Given_A_Valid_String
    {
        [Theory]
        [InlineData("V000001")]
        [InlineData("V000002")]
        [InlineData("V845685")]
        [InlineData("V999999")]
        public void Given_Capital_V_Then_it_returns_a_new_vcode(string strCode)
        {
            var vCode = new VCode(strCode);

            vCode.Value.Should().Be(strCode);
        }

        [Theory]
        [InlineData("v000001","V000001")]
        [InlineData("v000002","V000002")]
        [InlineData("v458565","V458565")]
        [InlineData("v999999","V999999")]
        public void Given_LowerCase_v_Then_it_returns_a_new_vcode(string strCode, string expectedVCode)
        {
            var vCode = new VCode(strCode);

            vCode.Value.Should().Be(expectedVCode);
        }
    }

    public class Given_An_Invalid_String
    {
        [Theory]
        [InlineData("V00000000001")]
        [InlineData("V0000001")]
        [InlineData("V00001")]
        public void Given_Invalid_Length_Then_It_Throws_an_InvalidVCodeLengthException(string strCode)
        {
            var ctor = () => new VCode(strCode);
            ctor.Should().Throw<InvalidVCodeLength>();
        }

        [Theory]
        [InlineData("1234567")]
        [InlineData("A123456")]
        [InlineData("ABCDEFG")]
        [InlineData("VBCDEFG")]
        [InlineData("V12345A")]
        public void Given_It_Does_Not_Start_With_V_Then_It_Throws_an_InvalidVCodeFormatException(string strCode)
        {
            var ctor = () => new VCode(strCode);
            ctor.Should().Throw<InvalidVCodeFormat>();
        }
    }
}
