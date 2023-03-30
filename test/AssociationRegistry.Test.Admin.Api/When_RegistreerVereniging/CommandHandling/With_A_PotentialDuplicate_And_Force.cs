namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Events;
using Events.CommonEventDataTypes;
using AssociationRegistry.Framework;
using Locaties;
using Magda;
using Primitives;
using Fakes;
using Fixtures;
using Fixtures.Scenarios;
using Framework;
using Vereniging.DuplicateDetection;
using Vereniging.RegistreerVereniging;
using VerenigingsNamen;
using AutoFixture;
using FluentAssertions;
using Moq;
using ResultNet;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_PotentialDuplicate_And_Force : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase>>
{
    private readonly Result _result;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly RegistreerVerenigingCommand _command;
    private readonly RegistreerVerenigingCommand.Locatie _locatie;

    public With_A_PotentialDuplicate_And_Force(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase> classFixture)
    {
        var fixture = new Fixture().CustomizeAll();

        var duplicateChecker = new Mock<IDuplicateDetectionService>();
        var potentialDuplicates = new[] { fixture.Create<DuplicaatVereniging>() };
        duplicateChecker.Setup(
                d =>
                    d.GetDuplicates(
                        It.IsAny<VerenigingsNaam>(),
                        It.IsAny<LocatieLijst>()))
            .ReturnsAsync(potentialDuplicates);
        var today = fixture.Create<DateTime>();
        _locatie = fixture.Create<RegistreerVerenigingCommand.Locatie>() with { Postcode = classFixture.Scenario.Locatie.Postcode };

        _command = new RegistreerVerenigingCommand(
            classFixture.Scenario.Naam,
            null,
            null,
            NullOrEmpty<DateOnly>.Null,
            null,
            Array.Empty<AssociationRegistry.Vereniging.CommonCommandDataTypes.ContactInfo>(),
            new[] { _locatie },
            Array.Empty<RegistreerVerenigingCommand.Vertegenwoordiger>(),
            Array.Empty<string>(),
            SkipDuplicateDetection: true);

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingRepositoryMock = classFixture.VerenigingRepositoryMock;
        _vCodeService = new InMemorySequentialVCodeService();
        var commandHandler = new RegistreerVerenigingCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            Mock.Of<IMagdaFacade>(),
            duplicateChecker.Object,
            new ClockStub(today));

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
                null,
                _command.KboNummber,
                Array.Empty<ContactInfo>(),
                new[]
                {
                    new VerenigingWerdGeregistreerd.Locatie(
                        _locatie.Naam,
                        _locatie.Straatnaam,
                        _locatie.Huisnummer,
                        _locatie.Busnummer,
                        _locatie.Postcode,
                        _locatie.Gemeente,
                        _locatie.Land,
                        _locatie.Hoofdlocatie,
                        _locatie.Locatietype),
                },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()
            ));
    }
}
