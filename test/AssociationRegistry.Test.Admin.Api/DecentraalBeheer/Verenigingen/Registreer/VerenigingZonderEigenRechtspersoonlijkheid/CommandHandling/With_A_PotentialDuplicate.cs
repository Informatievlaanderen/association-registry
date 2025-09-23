namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Wolverine.Marten;
using Xunit;

public class With_A_PotentialDuplicate
{
    private readonly Result<PotentialDuplicatesFound> _potentialDuplicates;
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
        };

        _potentialDuplicates = PotentialDuplicatesFound.Some(fixture.Create<string>(), fixture.Create<DuplicaatVereniging>());

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            new VerenigingRepositoryMock(scenario.GetVerenigingState()),
            new InMemorySequentialVCodeService(),
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            Mock.Of<IGeotagsService>(),
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        _result = commandHandler.Handle(
                                     new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                         command, commandMetadata),
                                     VerrijkteAdressenUitGrar.Empty,
                                     _potentialDuplicates,
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
        ((Result<PotentialDuplicatesFound>)_result).Should().BeEquivalentTo(
            new Result<PotentialDuplicatesFound>(_potentialDuplicates, ResultStatus.Failed),
            options => options.Excluding(x => x.LogTraceCode));
    }
}
