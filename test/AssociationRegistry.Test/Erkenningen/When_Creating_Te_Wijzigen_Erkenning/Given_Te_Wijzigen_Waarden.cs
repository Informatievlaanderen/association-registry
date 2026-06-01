namespace AssociationRegistry.Test.Erkenningen.When_Creating_Te_Wijzigen_Erkenning;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Primitives;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Te_Wijzigen_Waarden
{
    private readonly Fixture _fixture;

    public Given_Te_Wijzigen_Waarden()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public void With_Startdatum_Filled_In_Then_Creates()
    {
        var startdatum = NullOrEmpty<DateOnly>.Create(_fixture.Create<DateOnly>());
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            startdatum,
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            null,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }

    [Fact]
    public void With_Startdatum_Is_Filled_Empty_Then_Creates()
    {
        var startdatum = NullOrEmpty<DateOnly>.Empty;
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            startdatum,
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            null,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }

    [Fact]
    public void With_Startdatum_Is_Filled_With_Random_Value_Then_Creates()
    {
        var startdatum = NullOrEmpty<DateOnly>.Create(_fixture.Create<DateOnly>());
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            startdatum,
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            null,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }

    [Fact]
    public void With_EindDatum_Is_Filled_In_Then_Creates()
    {
        var eindDatum = NullOrEmpty<DateOnly>.Create(_fixture.Create<DateOnly>());
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            NullOrEmpty<DateOnly>.Null,
            eindDatum,
            NullOrEmpty<DateOnly>.Null,
            null,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }

    [Fact]
    public void With_EindDatum_Is_Filled_In_Empty_Then_Creates()
    {
        var eindDatum = NullOrEmpty<DateOnly>.Empty;
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            NullOrEmpty<DateOnly>.Null,
            eindDatum,
            NullOrEmpty<DateOnly>.Null,
            null,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }

    [Fact]
    public void With_HernieuwingsDatum_Is_Filled_In_Then_Creates()
    {
        var hernieuwingsDatum = NullOrEmpty<DateOnly>.Create(_fixture.Create<DateOnly>());
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            hernieuwingsDatum,
            null,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }

    [Fact]
    public void With_HernieuwingsDatum_Is_Filled_In_Empty_Then_Creates()
    {
        var hernieuwingsDatum = NullOrEmpty<DateOnly>.Empty;
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            hernieuwingsDatum,
            null,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }

    [Fact]
    public void With_HernieuwingsUrl_Is_Filled_In_Then_Creates()
    {
        var hernieuwingsUrl = _fixture.Create<string>();
        var redenVanWijziging = _fixture.Create<string>();

        var erkenning = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            hernieuwingsUrl,
            redenVanWijziging);

        erkenning.Should().NotBeNull();
    }
}
