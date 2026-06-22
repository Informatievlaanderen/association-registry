namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_No_RedenVanWijziging
{
    private readonly WijzigErkenningContext<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario(),
            s => s.ErkenningWerdGeregistreerd.ErkenningId,
            s => s.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async ValueTask Then_It_Throws_RedenVanWijzigingIsVerplicht(string redenVanWijziging)
    {
        var erkenning = _ctx.CreateTeWijzigenErkenning() with
        {
            StartDatum = NullOrEmpty<DateOnly>.Null,
            EindDatum = NullOrEmpty<DateOnly>.Null,
            Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
            HernieuwingsUrl = string.Empty,
            RedenVanWijziging = redenVanWijziging,
        };
        var command = _ctx.WijzigErkenningCommand with
        {
            Erkenning = erkenning,
        };

        var exception = await Assert.ThrowsAsync<RedenVanWijzigingIsVerplicht>(async () => await _ctx.Handle(command));

        exception.Message.Should().Be(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }
}
