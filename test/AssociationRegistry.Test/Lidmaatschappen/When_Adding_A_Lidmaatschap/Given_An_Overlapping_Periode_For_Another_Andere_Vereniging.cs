namespace AssociationRegistry.Test.Lidmaatschappen.When_Adding_A_Lidmaatschap;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Lidmaatschappen.VoegLidmaatschapToe;
using FluentAssertions;
using Xunit;

public class Given_An_Overlapping_Periode_For_Another_Andere_Vereniging
{
    [Fact]
    public void Then_It_Returns_With_Next_Id()
    {
        var fixture = new Fixture().CustomizeDomain();

        var andereVereniging = fixture.Create<VCode>();

        var bestaandLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            LidmaatschapId = new LidmaatschapId(1),
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = Geldigheidsperiode.Infinity,
        };

        var toeTeVoegenLidmaatschap = fixture.Create<VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap>() with
        {
            AndereVereniging = fixture.Create<VCode>(),
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
        ]);

        var actual = sut.VoegToe(toeTeVoegenLidmaatschap);

        actual.Should().BeEquivalentTo(
            Lidmaatschap.Hydrate(new LidmaatschapId(bestaandLidmaatschap.LidmaatschapId + 1),
                                 toeTeVoegenLidmaatschap.AndereVereniging,
                                 toeTeVoegenLidmaatschap.AndereVerenigingNaam,
                                 toeTeVoegenLidmaatschap.Geldigheidsperiode,
                                 toeTeVoegenLidmaatschap.Identificatie,
                                 toeTeVoegenLidmaatschap.Beschrijving));
    }
}
