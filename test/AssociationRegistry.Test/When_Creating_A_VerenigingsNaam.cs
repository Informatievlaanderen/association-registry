namespace AssociationRegistry.Test;

using VerenigingsNamen;
using VerenigingsNamen.Exceptions;
using FluentAssertions;
using Xunit;

public class When_Creating_A_VerenigingsNaam
{
    public class Given_A_Naam
    {
        [Theory]
        [InlineData("Vereniging A")]
        [InlineData("Jabedabedoe")]
        [InlineData("Vereniging zonder naam")]
        [InlineData("123456789")]
        public void Then_It_Returns_A_New_VerenigingsNaam(string naam)
        {
            var verenigingsNaam = new VerenigingsNaam(naam);
            verenigingsNaam.ToString().Should().Be(naam);
        }
    }

    public class Given_An_Empty_String
    {
        [Fact]
        public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
        {
            var ctor = () => new VerenigingsNaam(String.Empty);
            ctor.Should().Throw<EmptyVerenigingsNaam>();
        }
    }

    public class Given_Null
    {
        [Fact]
        public void Then_It_Throws_An_EmptyVerenigingsNaam_Exception()
        {
            var ctor = () => new VerenigingsNaam(null!);
            ctor.Should().Throw<EmptyVerenigingsNaam>();
        }
    }
}
