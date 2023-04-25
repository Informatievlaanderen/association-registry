namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using DuplicateVerenigingDetection;
using Fakes;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using Framework.MagdaMocks;
using Moq;
using ResultNet;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_PotentialDuplicate
{
    private readonly List<DuplicaatVereniging> _potentialDuplicates;
    private readonly Result _result;

    public With_A_PotentialDuplicate()
    {
        var scenario = new VerenigingWerdGeregistreerdWithLocationScenario();
        var fixture = new Fixture().CustomizeAll();

        var locatie = fixture.Create<Locatie>() with { Postcode = scenario.Locatie.Postcode };
        var command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Naam = VerenigingsNaam.Create(VerenigingWerdGeregistreerdWithLocationScenario.Naam),
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
            new VerenigingRepositoryMock(scenario.GetVereniging()),
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
