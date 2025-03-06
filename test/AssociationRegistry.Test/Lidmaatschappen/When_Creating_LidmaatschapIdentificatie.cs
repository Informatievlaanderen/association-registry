namespace AssociationRegistry.Test.Lidmaatschappen;

using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using System;
using Xunit;

public class When_Creating_LidmaatschapIdentificatie
{
    [Fact]
    public void Value_Cannot_Be_Null()
    {
        Assert.Throws<ArgumentNullException>(() => LidmaatschapIdentificatie.Create(null));
    }

    [Fact]
    public void Value_Can_Be_Empty()
    {
        var lidmaatschapBeschrijving = LidmaatschapIdentificatie.Create(string.Empty);

        lidmaatschapBeschrijving.Should().NotBeNull();
        lidmaatschapBeschrijving.Should().BeEquivalentTo(LidmaatschapIdentificatie.Hydrate(string.Empty));
    }

    [Fact]
    public void Value_Can_Be_AnyString()
    {
        var beschrijving = new Fixture().Create<string>();

        var lidmaatschapBeschrijving = LidmaatschapIdentificatie.Create(beschrijving);

        lidmaatschapBeschrijving.Should().NotBeNull();
        lidmaatschapBeschrijving.Should().BeEquivalentTo(LidmaatschapIdentificatie.Hydrate(beschrijving));
    }
}
