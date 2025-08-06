namespace AssociationRegistry.Test.Lidmaatschappen;

using AssociationRegistry.Vereniging;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using Xunit;

public class When_Creating_LidmaatschapBeschrijving
{
    [Fact]
    public void Value_Cannot_Be_Null()
    {
        Assert.Throws<ArgumentNullException>(() => LidmaatschapBeschrijving.Create(null));
    }

    [Fact]
    public void Value_Can_Be_Empty()
    {
        var lidmaatschapBeschrijving = LidmaatschapBeschrijving.Create(string.Empty);

        lidmaatschapBeschrijving.Should().NotBeNull();
        lidmaatschapBeschrijving.Should().BeEquivalentTo(LidmaatschapBeschrijving.Hydrate(string.Empty));
    }

    [Fact]
    public void Value_Can_Be_AnyString()
    {
        var beschrijving = new Fixture().Create<string>();

        var lidmaatschapBeschrijving = LidmaatschapBeschrijving.Create(beschrijving);

        lidmaatschapBeschrijving.Should().NotBeNull();
        lidmaatschapBeschrijving.Should().BeEquivalentTo(LidmaatschapBeschrijving.Hydrate(beschrijving));
    }
}
