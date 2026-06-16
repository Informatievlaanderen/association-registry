namespace AssociationRegistry.Test.Erkenningen.When_Creating_Te_Wijzigen_Erkenning;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Primitives;
using Xunit;

public class Given_Geen_Te_Wijzigen_Waarden
{
    private readonly Fixture _fixture;

    public Given_Geen_Te_Wijzigen_Waarden()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public void Then_Creates_TeWijzigenErkenning_With_HeeftGeenTeWijzigenWaarde_True()
    {
        var redenVanWijziging = _fixture.Create<string>();
        var result = TeWijzigenErkenning.Create(
            _fixture.Create<int>(),
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            NullOrEmpty<DateOnly>.Null,
            null,
            redenVanWijziging);

        result.HeeftGeenTeWijzigenWaarde.Should().BeTrue();
    }
}
