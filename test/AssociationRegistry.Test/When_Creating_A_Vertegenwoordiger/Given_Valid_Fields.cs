namespace AssociationRegistry.Test.When_Creating_A_Vertegenwoordiger;

using FluentAssertions;
using FluentAssertions.Execution;
using INSZ;
using Vertegenwoordigers;
using Xunit;

public class Given_Valid_Fields
{
    [Theory]
    [InlineData("00000000000", "joske", "teamlead", true)]
    public void Then_It_Returns_A_Vertegenwoordiger(string inszString, string roepnaam, string rol, bool primairContactpersoon)
    {
        var service = new VertegenwoordigersService();

        var insz = Insz.Create(inszString);
        var vertegenwoordiger = service.CreateVertegenwoordiger(insz, roepnaam, rol, primairContactpersoon);

        using (new AssertionScope())
        {
            vertegenwoordiger.Insz.Should().Be(insz);
            vertegenwoordiger.Voornaam.Should().Be("");
            vertegenwoordiger.Achternaam.Should().Be("");
            vertegenwoordiger.Roepnaam.Should().Be(roepnaam);
            vertegenwoordiger.Rol.Should().Be(rol);
            vertegenwoordiger.PrimairContactpersoon.Should().Be(primairContactpersoon);
        }
    }
}
