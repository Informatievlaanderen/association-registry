namespace AssociationRegistry.Test.Lidmaatschappen.When_Wijzig_A_Lidmaatschap;

using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using DecentraalBeheer.Lidmaatschappen.WijzigLidmaatschap;
using FluentAssertions;
using Xunit;
using Geldigheidsperiode = AssociationRegistry.Geldigheidsperiode;

public class Given_An_Overlapping_Periode_For_The_Same_Andere_Vereniging
{
    [Fact]
    public void Then_It_Throws()
    {
        var fixture = new Fixture().CustomizeDomain();

        var andereVereniging = fixture.Create<VCode>();
        var overlappendePeriode = new Geldigheidsperiode(new GeldigVan(1999, 10, 10), new GeldigTot(1999, 10, 10));

        var bestaandLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = overlappendePeriode,
        };

        var teWijzigenLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            LidmaatschapId = new LidmaatschapId(bestaandLidmaatschap.LidmaatschapId + 1),
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(2020, 10, 10), new GeldigTot(2020, 10, 10)),
        };

        var command = fixture.Create<WijzigLidmaatschapCommand.TeWijzigenLidmaatschap>() with
        {
            LidmaatschapId = teWijzigenLidmaatschap.LidmaatschapId,
            GeldigVan = overlappendePeriode.Van,
            GeldigTot = overlappendePeriode.Tot,
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
            teWijzigenLidmaatschap,
        ]);

        var exception = Assert.Throws<LidmaatschapIsOverlappend>(() => sut.Wijzig(command));
        exception.Message.Should().Be(ExceptionMessages.LidmaatschapIsOverlappend);
    }
}
