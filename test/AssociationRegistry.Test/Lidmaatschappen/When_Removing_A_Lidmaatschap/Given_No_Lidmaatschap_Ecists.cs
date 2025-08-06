namespace AssociationRegistry.Test.Lidmaatschappen.When_Removing_A_Lidmaatschap;

using AssociationRegistry.Resources;
using AssociationRegistry.Vereniging;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_No_Lidmaatschap_Exists
{
    [Fact]
    public void Then_It_Throws()
    {
        var lidmaatschapId = new LidmaatschapId(1);

        var sut = Lidmaatschappen.Empty;

        var exception = Assert.Throws<LidmaatschapIsNietGekend>(() => sut.Verwijder(lidmaatschapId));
        exception.Message.Should().Be(string.Format(ExceptionMessages.LidmaatschapIsNietGekend, lidmaatschapId));
    }
}
