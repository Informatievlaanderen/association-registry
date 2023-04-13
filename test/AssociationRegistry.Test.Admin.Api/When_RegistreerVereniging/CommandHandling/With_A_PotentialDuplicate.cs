namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
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
public class With_A_PotentialDuplicate : IClassFixture<CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase>>
{
    private readonly Result _result;
    private readonly List<DuplicaatVereniging> _potentialDuplicates;

    public With_A_PotentialDuplicate(CommandHandlerScenarioFixture<VerenigingWerdGeregistreerd_With_Location_Commandhandler_ScenarioBase> classFixture)
    {
        var fixture = new Fixture().CustomizeAll();

        var locatie = fixture.Create<Locatie>() with { Postcode = classFixture.Scenario.Locatie.Postcode };
        var command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Naam = new VerenigingsNaam(classFixture.Scenario.Naam),
            Locaties = new[] { locatie },
            SkipDuplicateDetection = false,
        };

        var duplicateChecker = new Mock<IDuplicateVerenigingDetectionService>();
        _potentialDuplicates = new List<DuplicaatVereniging> { fixture.Create<DuplicaatVereniging>() };
        duplicateChecker.Setup(
                d =>
                    d.GetDuplicates(
                        command.Naam,
                        command.Locaties))
            .ReturnsAsync(_potentialDuplicates);

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(
            classFixture.VerenigingRepositoryMock,
            new InMemorySequentialVCodeService(),
            new MagdaFacadeEchoMock(),
            duplicateChecker.Object,
            new ClockStub(command.Startdatum.Datum!.Value));

        _result = commandHandler.Handle(new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_The_Result_Is_A_Failure()
    {
        _result.IsFailure().Should().BeTrue();
    }

    [Fact]
    public void Then_The_Result_Contains_The_Potential_Duplicates()
    {
        ((Result<PotentialDuplicatesFound>)_result).Data.Candidates.Should().BeEquivalentTo(_potentialDuplicates);
    }
}
