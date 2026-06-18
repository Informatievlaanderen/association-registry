namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_No_TeWijzigen_Values
{
    private readonly WijzigErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Fact]
    public async ValueTask With_RedenVanWijziging_Then_It_Throws_MinstensEenTeWijzigenVeldMoetIngevuldZijn()
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            EindDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
            HernieuwingsUrl = null,
        };
        var command = _ctx.CreateCommand(teWijzigenErkenning: erkenning);

        var exception = await Assert.ThrowsAsync<MinstensEenTeWijzigenVeldMoetIngevuldZijn>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.MinstensEenTeWijzigenVeldMoetIngevuldZijn);
    }
}
