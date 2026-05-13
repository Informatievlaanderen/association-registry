namespace AssociationRegistry.Test.Erkenningen.When_Creating_Te_Corrigeren_Erkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Primitives;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Geen_Te_Corrigeren_Waarden
{
    private readonly Fixture _fixture;

    public Given_Geen_Te_Corrigeren_Waarden()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public void Then_Throws_TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben()
    {
        var exception = Assert.Throws<TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben>(() => TeCorrigerenErkenning.Create(
            _fixture.Create<int>(),
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            null));

        exception.Message.Should().Be(ExceptionMessages.TeCorrigerenErkenningMoetMinstensEenTeCorrigerenWaardeHebben);
    }
}
