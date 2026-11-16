namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

/// <summary>
/// SubverenigingRelatieWerdGewijzigd only changes the koepel: identificatie and beschrijving stay.
/// </summary>
public class Given_SubverenigingRelatieWerdGewijzigd
{
    private readonly Fixture _fixture;
    private readonly VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario _scenario;
    private readonly SubverenigingRelatieWerdGewijzigd _relatieWerdGewijzigd;
    private readonly MultipleVerenigingAggregateSessionMock _aggregateSessionMock;
    private readonly VerfijnSubtypeNaarSubverenigingCommandHandler _commandHandler;

    public Given_SubverenigingRelatieWerdGewijzigd()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingssubtypeWerdVerfijndNaarSubverenigingScenario();
        var koepelScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _relatieWerdGewijzigd = new SubverenigingRelatieWerdGewijzigd(
            _scenario.VCode,
            koepelScenario.VCode,
            koepelScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam
        );

        _aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(
            _scenario.GetVerenigingState().Apply(_relatieWerdGewijzigd)
        );
        _commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(_aggregateSessionMock);
    }

    private Registratiedata.SubverenigingVan Verfijnd =>
        _scenario.VerenigingssubtypeWerdVerfijndNaarSubvereniging.SubverenigingVan;

    [Fact]
    public void Then_The_State_Keeps_Identificatie_And_Beschrijving()
    {
        _scenario
            .GetVerenigingState()
            .Apply(_relatieWerdGewijzigd)
            .Verenigingssubtype.Should()
            .Be(
                new SubverenigingSubtype(
                    new SubverenigingVan
                    {
                        AndereVereniging = _relatieWerdGewijzigd.AndereVereniging,
                        AndereVerenigingNaam = _relatieWerdGewijzigd.AndereVerenigingNaam,
                        Identificatie = Verfijnd.Identificatie,
                        Beschrijving = Verfijnd.Beschrijving,
                    }
                )
            );
    }

    [Fact]
    public async ValueTask With_Only_A_Beschrijving_Then_SubverenigingDetailsWerdenGewijzigd_Keeps_The_Identificatie()
    {
        var beschrijving = SubverenigingBeschrijving.Create(_fixture.Create<string>());

        var command = new VerfijnSubtypeNaarSubverenigingCommand(
            _scenario.VCode,
            new SubverenigingVanDto(null, null, beschrijving)
        );

        await _commandHandler.Handle(
            new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldHaveSaved(
            _scenario.VCode,
            new SubverenigingDetailsWerdenGewijzigd(_scenario.VCode, Verfijnd.Identificatie, beschrijving)
        );
    }

    [Fact]
    public async ValueTask With_The_Same_Details_Then_Nothing()
    {
        var command = new VerfijnSubtypeNaarSubverenigingCommand(
            _scenario.VCode,
            new SubverenigingVanDto(
                null,
                SubverenigingIdentificatie.Create(Verfijnd.Identificatie),
                SubverenigingBeschrijving.Create(Verfijnd.Beschrijving)
            )
        );

        await _commandHandler.Handle(
            new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldNotHaveAnySaves(_scenario.VCode);
    }
}
