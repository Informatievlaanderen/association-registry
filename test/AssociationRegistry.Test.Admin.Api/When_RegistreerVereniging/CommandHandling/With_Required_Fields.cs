﻿namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Events;
using AssociationRegistry.Framework;
using Magda;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Framework;
using Moq;
using Primitives;
using Startdatums;
using VerenigingsNamen;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Required_Fields : IClassFixture<CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase>>
{
    private const string Naam = "naam1";

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Required_Fields(CommandHandlerScenarioFixture<Empty_Commandhandler_ScenarioBase> classFixture)
    {
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAll();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var command = fixture.Create<RegistreerVerenigingCommand>() with { Naam = new VerenigingsNaam(Naam) };
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, Mock.Of<IMagdaFacade>(), new NoDuplicateDetectionService(), clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata), CancellationToken.None)
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
                Startdatum: null,
                KboNummer: null,
                Contactgegevens: Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Locaties: Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Vertegenwoordigers: Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                HoofdactiviteitenVerenigingsloket: Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()));
    }
}
