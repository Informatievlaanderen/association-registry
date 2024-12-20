namespace AssociationRegistry.Test.Lidmaatschappen.When_Wijzig_A_Lidmaatschap;

using AssociationRegistry.Acties.WijzigLidmaatschap;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_An_Overlapping_Periode_For_Another_Andere_Vereniging
{
    [Fact]
    public void Then_It_Updates()
    {
        var fixture = new Fixture().CustomizeDomain();

        var andereVereniging = fixture.Create<VCode>();
        var andereVereniging2 = fixture.Create<VCode>();
        var overlappendePeriode = new Geldigheidsperiode(new GeldigVan(1999, 10, 10), new GeldigTot(1999, 10, 10));

        var lidmaatschapVoorAndereVereniging = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = overlappendePeriode,
        };

        var teWijzigenLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging2,
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(2020, 10, 10), new GeldigTot(2020, 10, 10)),
        };

        var command = fixture.Create<WijzigLidmaatschapCommand.TeWijzigenLidmaatschap>() with
        {
            LidmaatschapId = teWijzigenLidmaatschap.LidmaatschapId,
            GeldigVan = overlappendePeriode.Van,
            GeldigTot = overlappendePeriode.Tot,
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            lidmaatschapVoorAndereVereniging,
            teWijzigenLidmaatschap,
        ]);

        var actual = sut.Wijzig(command);

        actual.Should().BeEquivalentTo(Lidmaatschap.Hydrate(
                                           new LidmaatschapId(command.LidmaatschapId ),
                                           teWijzigenLidmaatschap.AndereVereniging,
                                           teWijzigenLidmaatschap.AndereVerenigingNaam,
                                           new Geldigheidsperiode(command.GeldigVan.Value, command.GeldigTot.Value),
                                           command.Identificatie,
                                           command.Beschrijving));
    }
}
