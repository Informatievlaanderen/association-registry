namespace AssociationRegistry.Test.Lidmaatschappen.When_Wijzig_A_Lidmaatschap;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.WijzigLidmaatschap;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;
using Geldigheidsperiode = AssociationRegistry.Geldigheidsperiode;

public class Given_No_Overlapping_Periode_For_The_Same_Andere_Vereniging
{
    [Fact]
    public void Then_It_Updates()
    {
        var fixture = new Fixture().CustomizeDomain();

        var andereVereniging = fixture.Create<VCode>();

        var bestaandLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(1999, 10, 10), new GeldigTot(1999, 10, 10)),
        };

        var teWijzigenLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(2020, 10, 10), new GeldigTot(2020, 10, 10)),
        };

        var command = fixture.Create<TeWijzigenLidmaatschap>() with
        {
            LidmaatschapId = teWijzigenLidmaatschap.LidmaatschapId,
            GeldigVan = new GeldigVan(2015, 10, 10),
            GeldigTot = new GeldigTot(2015, 10, 10),
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
            teWijzigenLidmaatschap,
        ]);

        var actual = sut.Wijzig(command);

        actual.Should().BeEquivalentTo(Lidmaatschap.Hydrate(
                                           new LidmaatschapId(command.LidmaatschapId),
                                           teWijzigenLidmaatschap.AndereVereniging,
                                           teWijzigenLidmaatschap.AndereVerenigingNaam,
                                           new Geldigheidsperiode(command.GeldigVan.Value,
                                                                  command.GeldigTot.Value),
                                           command.Identificatie,
                                           command.Beschrijving));
    }
}
