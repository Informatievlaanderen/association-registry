namespace AssociationRegistry.Test.Lidmaatschappen;

using Acties.Lidmaatschappen.VoegLidmaatschapToe;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Xunit;
using Geldigheidsperiode = AssociationRegistry.Geldigheidsperiode;

public class EqualsTest
{
    [Fact]
    public void With_The_Same_Values_Return_True()
    {
        var fixture = new Fixture().CustomizeDomain();
        var lidmaatschap1 = fixture.Create<Lidmaatschap>();

        var lidmaatschap2 = Lidmaatschap.Create(
            lidmaatschap1.LidmaatschapId,
            new VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap(lidmaatschap1.AndereVereniging,
                                                                   lidmaatschap1.AndereVerenigingNaam,
                                                                   new Geldigheidsperiode(
                                                                       new GeldigVan(lidmaatschap1.Geldigheidsperiode.Van),
                                                                       new GeldigTot(lidmaatschap1.Geldigheidsperiode.Tot)),
                                                                   lidmaatschap1.Identificatie,
                                                                   lidmaatschap1.Beschrijving));

        Assert.True(lidmaatschap1.Equals(lidmaatschap2));
    }

    [Fact]
    public void With_Other_Van_Value_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var lidmaatschap1 = fixture.Create<Lidmaatschap>();

        var lidmaatschap2 = lidmaatschap1 with
        {
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(), new GeldigTot(lidmaatschap1.Geldigheidsperiode.Tot)),
        };

        Assert.False(lidmaatschap1.Equals(lidmaatschap2));
    }

    [Fact]
    public void With_Other_Tot_Value_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var lidmaatschap1 = fixture.Create<Lidmaatschap>();

        var lidmaatschap2 = lidmaatschap1 with
        {
            Geldigheidsperiode = new Geldigheidsperiode(new GeldigVan(lidmaatschap1.Geldigheidsperiode.Van), new GeldigTot())
        };

        Assert.False(lidmaatschap1.Equals(lidmaatschap2));
    }

    [Fact]
    public void With_Other_Beschrijving_Value_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var lidmaatschap1 = fixture.Create<Lidmaatschap>();

        var lidmaatschap2 = lidmaatschap1 with
        {
            Beschrijving = fixture.Create<LidmaatschapBeschrijving>(),
        };

        Assert.False(lidmaatschap1.Equals(lidmaatschap2));
    }

    [Fact]
    public void With_Other_Identificatie_Value_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var lidmaatschap1 = fixture.Create<Lidmaatschap>();

        var lidmaatschap2 = lidmaatschap1 with
        {
            Identificatie = fixture.Create<LidmaatschapIdentificatie>(),
        };

        Assert.False(lidmaatschap1.Equals(lidmaatschap2));
    }

    [Fact]
    public void With_Other_AndereVereniging_Value_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var lidmaatschap1 = fixture.Create<Lidmaatschap>();

        var lidmaatschap2 = lidmaatschap1 with
        {
            AndereVereniging = fixture.Create<VCode>(),
        };

        Assert.False(lidmaatschap1.Equals(lidmaatschap2));
    }

    [Fact]
    public void With_Other_AndereVerenigingNaam_Value_Return_False()
    {
        var fixture = new Fixture().CustomizeDomain();
        var lidmaatschap1 = fixture.Create<Lidmaatschap>();

        var lidmaatschap2 = lidmaatschap1 with
        {
            AndereVerenigingNaam = fixture.Create<string>(),
        };

        Assert.False(lidmaatschap1.Equals(lidmaatschap2));
    }
}
