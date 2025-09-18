namespace AssociationRegistry.Test.Lidmaatschappen.When_Adding_A_Lidmaatschap;

using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Xunit;

public class Given_An_Overlapping_Periode_For_The_Same_Andere_Vereniging
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var andereVereniging = fixture.Create<VCode>();
        var bestaandLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = Geldigheidsperiode.Infinity,
        };
        var toeTeVoegenLidmaatschap = fixture.Create<ToeTeVoegenLidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
        ]);

        var exception = Assert.Throws<LidmaatschapIsOverlappend>(() => sut.VoegToe(toeTeVoegenLidmaatschap));
        exception.Message.Should().Be(ExceptionMessages.LidmaatschapIsOverlappend);
    }
}
