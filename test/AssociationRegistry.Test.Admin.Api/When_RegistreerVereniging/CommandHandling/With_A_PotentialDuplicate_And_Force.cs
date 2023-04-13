namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using AutoFixture;
using DuplicateVerenigingDetection;
using FluentAssertions;
using Framework.MagdaMocks;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_PotentialDuplicate_And_Force : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase>>
{
    private readonly Result _result;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly RegistreerVerenigingCommand _command;
    private readonly Locatie _locatie;

    public With_A_PotentialDuplicate_And_Force(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase> classFixture)
    {
        var fixture = new Fixture().CustomizeAll();

        _locatie = fixture.Create<Locatie>() with { Postcode = classFixture.Scenario.Locatie.Postcode };
        _command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Naam = new VerenigingsNaam(classFixture.Scenario.Naam),
            Locaties = new[] { _locatie },
            SkipDuplicateDetection = true,
        };

        var duplicateChecker = new Mock<IDuplicateVerenigingDetectionService>();
        var potentialDuplicates = new[] { fixture.Create<DuplicaatVereniging>() };
        duplicateChecker.Setup(
                d =>
                    d.GetDuplicates(
                        _command.Naam,
                        _command.Locaties))
            .ReturnsAsync(potentialDuplicates);

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();
        var commandHandler = new RegistreerVerenigingCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new MagdaFacadeEchoMock(),
            duplicateChecker.Object,
            new ClockStub(_command.Startdatum.Datum!.Value));

        _result = commandHandler.Handle(new CommandEnvelope<RegistreerVerenigingCommand>(_command, commandMetadata), CancellationToken.None)
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
            new VerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.Naam,
                _command.KorteNaam,
                _command.KorteBeschrijving,
                _command.Startdatum,
                _command.KboNummer!,
                _command.Contactgegevens.Select(
                    (g, index) => VerenigingWerdGeregistreerd.Contactgegeven.With(g) with
                    {
                        ContactgegevenId = index + 1,
                    }).ToArray(),
                new[]
                {
                    VerenigingWerdGeregistreerd.Locatie.With(_locatie),
                },
                _command.Vertegenwoordigers.Select(
                    v => VerenigingWerdGeregistreerd.Vertegenwoordiger.With(v) with
                    {
                        Contactgegevens = v.Contactgegevens.Select((c, index) => VerenigingWerdGeregistreerd.Contactgegeven.With(c) with
                        {
                            ContactgegevenId = index + 1,
                        }).ToArray(),
                        Voornaam = v.Insz, Achternaam = v.Insz,
                    }).ToArray(),
                _command.HoofdactiviteitenVerenigingsloket.Select(
                        h => new VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket(
                            h.Code, h.Beschrijving)).ToArray()
            ));
    }
}
