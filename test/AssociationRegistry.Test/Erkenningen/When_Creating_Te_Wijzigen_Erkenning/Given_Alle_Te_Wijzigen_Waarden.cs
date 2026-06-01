namespace AssociationRegistry.Test.Erkenningen.When_Creating_Te_Wijzigen_Erkenning;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Primitives;
using Xunit;

public class Given_Alle_Te_Wijzigen_Waarden
{
    private readonly Fixture _fixture;

    public Given_Alle_Te_Wijzigen_Waarden()
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
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            erkenningId,
            startDatum,
            eindDatum,
            hernieuwingsDatum,
            hernieuwingsUrl,
            redenVanWijziging);

        erkenning.ErkenningId.Should().Be(erkenningId);
        erkenning.StartDatum.Should().Be(startDatum);
        erkenning.EindDatum.Should().Be(eindDatum);
        erkenning.Hernieuwingsdatum.Should().Be(hernieuwingsDatum);
        erkenning.HernieuwingsUrl.Should().Be(hernieuwingsUrl);
        erkenning.RedenVanWijziging.Should().Be(redenVanWijziging);
    }
}
