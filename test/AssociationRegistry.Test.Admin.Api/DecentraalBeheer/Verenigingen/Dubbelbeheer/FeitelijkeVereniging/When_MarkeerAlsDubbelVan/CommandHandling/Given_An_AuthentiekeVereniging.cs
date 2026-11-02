namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Xunit;

/// <summary>
///     Een vereniging die eerder als authentiek fungeerde (A is dubbel van B) mag alsnog dubbel worden (B is dubbel van C).
///     De dubbele vereniging heeft geen basisgegevens, hoofdactiviteiten of contactgegevens, dus er valt niets samen te voegen.
///     De samenvoeging zelf wordt getest in <see cref="Given_Basisgegevens" />.
/// </summary>
public class Given_An_Authentieke_Vereniging
{
    private readonly Fixture _fixture;
    private readonly VerenigingAanvaarddeDubbeleVerenigingScenario _scenario;
    private readonly MultipleVerenigingAggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(
            VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke
        );

        var nieuweAuthentiekeVereniging =
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        var dubbeleVereniging = _scenario
            .GetVerenigingState()
            .Apply(new KorteNaamWerdGewijzigd(_scenario.VCode, KorteNaam: ""))
            .Apply(new KorteBeschrijvingWerdGewijzigd(_scenario.VCode, KorteBeschrijving: ""))
            .Apply(new StartdatumWerdGewijzigd(_scenario.VCode, Startdatum: null))
            .Apply(new DoelgroepWerdGewijzigd(TestBasisgegevens.Empty.Doelgroep))
            .Apply(new HoofdactiviteitenVerenigingsloketWerdenGewijzigd([]));

        var dubbeleVerenigingZonderGegevens = dubbeleVereniging.Contactgegevens.Aggregate(
            dubbeleVereniging,
            (state, contactgegeven) =>
                state.Apply(
                    new ContactgegevenWerdVerwijderd(
                        contactgegeven.ContactgegevenId,
                        contactgegeven.Contactgegeventype,
                        contactgegeven.Waarde,
                        contactgegeven.Beschrijving,
                        contactgegeven.IsPrimair
                    )
                )
        );

        _aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(dubbeleVerenigingZonderGegevens);
        _aggregateSessionMock.WithVereniging(new VerenigingState().Apply(nieuweAuthentiekeVereniging));

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _aggregateSessionMock,
            new VerenigingsStateQueriesMock()
        );

        _command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = VCode.Create(nieuweAuthentiekeVereniging.VCode),
        };
    }

    private async Task Handle() =>
        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>())
        );

    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingWerdGemarkeerdAlsDubbelVan()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _scenario.VCode,
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, _command.VCodeAuthentiekeVereniging)
        );
    }

    [Fact]
    public async ValueTask Then_The_New_Authentieke_Vereniging_Aanvaardt_The_Dubbel()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _command.VCodeAuthentiekeVereniging,
            new VerenigingAanvaarddeDubbeleVereniging(_command.VCodeAuthentiekeVereniging, _scenario.VCode)
        );
    }
}
