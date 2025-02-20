namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.CommandHandling;

using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using DuplicateVerenigingDetection;
using AssociationRegistry.Framework;
using Grar.Clients;
using Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
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
        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithLocationScenario();
        var fixture = new Fixture().CustomizeAdminApi();

        var locatie = fixture.Create<Locatie>() with
        {
            AdresId = null,
        };

        locatie.Adres!.Postcode = scenario.Locatie.Adres!.Postcode;

        var command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Naam = VerenigingsNaam.Create(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithLocationScenario.Naam),
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

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            new VerenigingRepositoryMock(scenario.GetVerenigingState()),
            new InMemorySequentialVCodeService(),
            duplicateChecker.Object,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            Mock.Of<IGrarClient>(),
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        _result = commandHandler.Handle(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata),
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
