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
using Common.StubsMocksFakes;
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

        var locatie = fixture.Create<Locatie>() with { AdresId = null };

        locatie.Adres!.Postcode = scenario.Locatie.Adres!.Postcode;

        var command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Naam = VerenigingsNaam.Create(
                naam: VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithLocationScenario.Naam
            ),
            Locaties = [locatie],
        };

        _potentialDuplicates = PotentialDuplicatesFound.Some(
            newBevestigingstoken: fixture.Create<string>(),
            potentialDuplicates: fixture.Create<DuplicaatVereniging>()
        );

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: new NewAggregateSessionMock(verenigingToLoad: scenario.GetVerenigingState()),
            vCodeService: new InMemorySequentialVCodeService(),
            outbox: Mock.Of<IMartenOutbox>(),
            session: Mock.Of<IDocumentSession>(),
            clock: new ClockStub(now: command.Startdatum.Value),
            geotagsService: Mock.Of<IGeotagsService>(),
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );

        _result = commandHandler
            .Handle(
                message: new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                    Command: command,
                    Metadata: commandMetadata
                ),
                verrijkteAdressenUitGrar: VerrijkteAdressenUitGrar.Empty,
                potentialDuplicates: _potentialDuplicates,
                personenUitKsz: new PersonenUitKszStub(command: command),
                cancellationToken: CancellationToken.None
            )
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
        ((Result<PotentialDuplicatesFound>)_result)
            .Should()
            .BeEquivalentTo(
                expectation: new Result<PotentialDuplicatesFound>(
                    data: _potentialDuplicates,
                    status: ResultStatus.Failed
                ),
                config: options => options.Excluding(expression: x => x.LogTraceCode)
            );
    }
}
