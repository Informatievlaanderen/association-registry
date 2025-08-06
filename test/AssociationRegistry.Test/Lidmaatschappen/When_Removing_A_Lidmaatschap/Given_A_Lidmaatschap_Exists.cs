namespace AssociationRegistry.Test.Lidmaatschappen.When_Removing_A_Lidmaatschap;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class Given_A_Lidmaatschap_Exists
{
    [Fact]
    public void Then_It_Returns_The_Removed_Lidmaatschap()
    {
        var fixture = new Fixture().CustomizeDomain();

        var andereVereniging = fixture.Create<VCode>();

        var bestaandLidmaatschap = fixture.Create<Lidmaatschap>() with
        {
            AndereVereniging = andereVereniging,
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(1999, 10, 10), new GeldigTot(1999, 10, 10)),
        };

        var sut = Lidmaatschappen.Empty.Hydrate([
            bestaandLidmaatschap,
        ]);

        var actual = sut.Verwijder(bestaandLidmaatschap.LidmaatschapId);

        actual.Should().Be(bestaandLidmaatschap);
    }
}
