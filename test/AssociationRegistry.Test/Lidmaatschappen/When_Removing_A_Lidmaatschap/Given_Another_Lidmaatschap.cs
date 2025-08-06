namespace AssociationRegistry.Test.Lidmaatschappen.When_Removing_A_Lidmaatschap;

using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_Another_Lidmaatschap
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();
        var bestaandLidmaatschap = fixture.Create<Lidmaatschap>();
        var nonExistingLidmaatschapId = new LidmaatschapId(bestaandLidmaatschap.LidmaatschapId + 1);

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
        ]);

        var exception = Assert.Throws<LidmaatschapIsNietGekend>(() => sut.Verwijder(nonExistingLidmaatschapId));
        exception.Message.Should().Be(string.Format(ExceptionMessages.LidmaatschapIsNietGekend, nonExistingLidmaatschapId));
    }
}
