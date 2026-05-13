namespace AssociationRegistry.Test.Erkenningen.When_Creating_Te_Corrigeren_Erkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Primitives;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Alle_Te_Corrigeren_Waarden
{
    private readonly Fixture _fixture;

    public Given_Alle_Te_Corrigeren_Waarden()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public void Then_All_Values_Are_Set()
    {
        var erkenningId = _fixture.Create<int>();
        var startDatum = NullOrEmpty<DateOnly>.Create(_fixture.Create<DateOnly>());
        var eindDatum = NullOrEmpty<DateOnly>.Create(_fixture.Create<DateOnly>());
        var hernieuwingsDatum = NullOrEmpty<DateOnly>.Create(_fixture.Create<DateOnly>());
        var hernieuwingsUrl = _fixture.Create<string>();

        var erkenning = TeCorrigerenErkenning.Create(
            erkenningId,
            startDatum,
            eindDatum,
            hernieuwingsDatum,
            hernieuwingsUrl);

        erkenning.ErkenningId.Should().Be(erkenningId);
        erkenning.StartDatum.Should().Be(startDatum);
        erkenning.EindDatum.Should().Be(eindDatum);
        erkenning.Hernieuwingsdatum.Should().Be(hernieuwingsDatum);
        erkenning.HernieuwingsUrl.Should().Be(hernieuwingsUrl);
    }
}
