namespace AssociationRegistry.Test.Lidmaatschappen.When_Adding_A_Lidmaatschap;

using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;
using Geldigheidsperiode = AssociationRegistry.Geldigheidsperiode;

public class Given_No_Overlapping_Periode_For_The_Same_Andere_Vereniging
{
    [Fact]
    public void Then_It_Adds()
    {
        var fixture = new Fixture().CustomizeDomain();

        var andereVereniging = fixture.Create<VCode>();

        var bestaandLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(1999, 10, 10), new GeldigTot(1999, 10, 10)),
        };

        var toeTeVoegenLidmaatschap = fixture.Create<ToeTeVoegenLidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(2020, 10, 10), new GeldigTot(2020, 10, 10)),
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
        ]);

        var actual = sut.VoegToe(toeTeVoegenLidmaatschap);

        actual.Should().BeEquivalentTo(Lidmaatschap.Hydrate(
                                           new LidmaatschapId(bestaandLidmaatschap.LidmaatschapId + 1),
                                           toeTeVoegenLidmaatschap.AndereVereniging,
                                           toeTeVoegenLidmaatschap.AndereVerenigingNaam,
                                           toeTeVoegenLidmaatschap.Geldigheidsperiode,
                                           toeTeVoegenLidmaatschap.Identificatie,
                                           toeTeVoegenLidmaatschap.Beschrijving));
    }
}
