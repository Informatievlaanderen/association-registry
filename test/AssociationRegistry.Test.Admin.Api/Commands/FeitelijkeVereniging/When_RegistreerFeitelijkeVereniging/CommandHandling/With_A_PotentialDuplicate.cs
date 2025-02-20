namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using DecentraalBeheer.Registratie.RegistreerFeitelijkeVereniging;
using DuplicateVerenigingDetection;
using FluentAssertions;
using Framework.Fakes;
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
public class With_A_PotentialDuplicate
{
    private readonly List<DuplicaatVereniging> _potentialDuplicates;
    private readonly Result _result;

    public With_A_PotentialDuplicate()
    {
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario();
        var fixture = new Fixture().CustomizeAdminApi();

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = null,
        };

        locatie.Adres!.Postcode = scenario.Locatie.Adres!.Postcode;

        var command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>() with
        {
            Naam = VerenigingsNaam.Create(FeitelijkeVerenigingWerdGeregistreerdWithLocationScenario.Naam),
            Locaties = new[] { locatie },
            SkipDuplicateDetection = false,
        };

        var duplicateChecker = new Mock<IDuplicateVerenigingDetectionService>();
        _potentialDuplicates = new List<DuplicaatVereniging> { fixture.Create<DuplicaatVereniging>() };

        duplicateChecker.Setup(
                             d =>
                                 d.GetDuplicates(
                                     command.Naam,
                                     command.Locaties,
                                     false, null))
                        .ReturnsAsync(_potentialDuplicates);

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(
            new VerenigingRepositoryMock(scenario.GetVerenigingState()),
            new InMemorySequentialVCodeService(),
            duplicateChecker.Object,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            Mock.Of<IGrarClient>(),
            NullLogger<RegistreerFeitelijkeVerenigingCommandHandler>.Instance);

        _result = commandHandler.Handle(new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(command, commandMetadata),
                                        CancellationToken.None)
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
