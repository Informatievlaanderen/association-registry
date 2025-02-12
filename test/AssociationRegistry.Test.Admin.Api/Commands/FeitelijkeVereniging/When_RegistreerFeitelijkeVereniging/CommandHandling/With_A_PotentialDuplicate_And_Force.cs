﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using DecentraalBeheer.Registratie.RegistreerFeitelijkeVereniging;
using DuplicateVerenigingDetection;
using EventFactories;
using Events;
using FluentAssertions;
using Framework.Fakes;
using Grar;
using Grar.Clients;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Vereniging;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_PotentialDuplicate_And_Force
{
    private readonly RegistreerFeitelijkeVerenigingCommand _command;
    private readonly Result _result;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_A_PotentialDuplicate_And_Force()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario();
        var fixture = new Fixture().CustomizeAdminApi()
                                   .WithoutWerkingsgebieden();

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = null,
        };

        locatie.Adres!.Postcode = scenario.Locatie.Adres!.Postcode;

        _command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>() with
        {
            Naam = VerenigingsNaam.Create(FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario.Naam),
            Locaties = new[] { locatie },
            SkipDuplicateDetection = true,
        };

        var notDistinct = _command.HoofdactiviteitenVerenigingsloket.Length !=
                          _command.HoofdactiviteitenVerenigingsloket.DistinctBy(x => x.Code).Count();

        var duplicateChecker = new Mock<IDuplicateVerenigingDetectionService>();
        var potentialDuplicates = new[] { fixture.Create<DuplicaatVereniging>() };

        duplicateChecker.Setup(
                             d =>
                                 d.GetDuplicates(
                                     _command.Naam,
                                     _command.Locaties,
                                     false,
                                     MinimumScore.Default))
                        .ReturnsAsync(potentialDuplicates);

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        _vCodeService = new InMemorySequentialVCodeService();

        var commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            duplicateChecker.Object,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(_command.Startdatum.Value),
            Mock.Of<IGrarClient>(),
            NullLogger<RegistreerFeitelijkeVerenigingCommandHandler>.Instance);

        _result = commandHandler.Handle(new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(_command, commandMetadata),
                                        CancellationToken.None)
                                .GetAwaiter()
                                .GetResult();
    }

    [Fact]
    public void Then_The_Result_Is_A_Success()
    {
        _result.IsSuccess().Should().BeTrue();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new FeitelijkeVerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.Naam,
                _command.KorteNaam ?? string.Empty,
                _command.KorteBeschrijving ?? string.Empty,
                _command.Startdatum,
                EventFactory.Doelgroep(_command.Doelgroep),
                _command.IsUitgeschrevenUitPubliekeDatastroom,
                _command.Contactgegevens.Select(
                    (g, index) => EventFactory.Contactgegeven(g) with
                    {
                        ContactgegevenId = index + 1,
                    }).ToArray(),
                _command.Locaties.Select(
                    (l, index) => EventFactory.Locatie(l) with
                    {
                        LocatieId = index + 1,
                    }).ToArray(),
                _command.Vertegenwoordigers.Select(
                    (v, index) => EventFactory.Vertegenwoordiger(v) with
                    {
                        VertegenwoordigerId = index + 1,
                    }).ToArray(),
                _command.HoofdactiviteitenVerenigingsloket.Select(
                    h => new Registratiedata.HoofdactiviteitVerenigingsloket(
                        h.Code,
                        h.Naam)).ToArray()
            ));
    }
}
