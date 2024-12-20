namespace AssociationRegistry.Test.Lidmaatschappen.When_Adding_A_Lidmaatschap;

using AssociationRegistry.Acties.VoegLidmaatschapToe;
using AssociationRegistry.Acties.WijzigLidmaatschap;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_First_Attempt_Failed
{
    [Fact]
    public void Then_It_Maintains_Correct_Next_LidmaatschapId()
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
            Geldigheidsperiode = Geldigheidsperiode.Infinity,
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
        ]);

        sut.Wijzig(fixture.Create<WijzigLidmaatschapCommand.TeWijzigenLidmaatschap>() with
        {
            LidmaatschapId = bestaandLidmaatschap.LidmaatschapId,
        });

        var lidmaatschap = sut.VoegToe(toeTeVoegenLidmaatschap);

        lidmaatschap.LidmaatschapId.Should().Be(new LidmaatschapId(2));
    }
}
