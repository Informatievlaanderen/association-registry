namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Magda;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Framework;
using Framework.MagdaMocks;
using Moq;
using Primitives;
using Startdatums;
using VerenigingsNamen;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_On_Today : IClassFixture<CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase>>
{
    private const string Naam = "naam1";

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly RegistreerVerenigingCommand _command;

    public With_A_Startdatum_On_Today(CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAll();

        _command = fixture.Create<RegistreerVerenigingCommand>() with { Naam = new VerenigingsNaam(Naam) };
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaFacadeEchoMock(),
            new NoDuplicateDetectionService(),
            new ClockStub(_command.Startdatum.Value!.Value));

        commandHandler
            .Handle(new CommandEnvelope<RegistreerVerenigingCommand>(_command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                VCode: _vCodeService.GetLast(),
                Naam: Naam,
                KorteNaam: null,
                KorteBeschrijving: null,
                Startdatum: _command.Startdatum,
                KboNummer: null,
                Contactgegevens: _command.Contactgegevens.Select(
                    (g, index) => VerenigingWerdGeregistreerd.Contactgegeven.With(g) with
                    {
                        ContactgegevenId = index + 1,
                    }).ToArray(),
                Locaties: Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Vertegenwoordigers: Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                HoofdactiviteitenVerenigingsloket: Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()));
    }
}
