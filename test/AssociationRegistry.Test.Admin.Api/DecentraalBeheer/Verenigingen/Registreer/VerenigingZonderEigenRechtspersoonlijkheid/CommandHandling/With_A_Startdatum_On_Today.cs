namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine.Marten;
using Xunit;

public class With_A_Startdatum_On_Today
{
    private const string Naam = "naam1";
    private readonly NewAggregateSessionMock _newAggregateSessionMock;
    private readonly GeotagsCollection _geotagsCollection;

    public With_A_Startdatum_On_Today()
    {
        _newAggregateSessionMock = new NewAggregateSessionMock();
        var vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Naam = VerenigingsNaam.Create(naam: Naam),
        };
        var commandMetadata = fixture.Create<CommandMetadata>();

        var (geotagService, geotagsCollection) = Faktory.New(fixture: fixture).GeotagsService.ReturnsRandomGeotags();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: _newAggregateSessionMock,
            vCodeService: vCodeService,
            outbox: Mock.Of<IMartenOutbox>(),
            session: Mock.Of<IDocumentSession>(),
            clock: new ClockStub(now: command.Startdatum.Value),
            geotagsService: geotagService.Object,
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );

        commandHandler
            .Handle(
                message: new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                    Command: command,
                    Metadata: commandMetadata
                ),
                verrijkteAdressenUitGrar: VerrijkteAdressenUitGrar.Empty,
                potentialDuplicates: PotentialDuplicatesFound.None,
                personenUitKsz: new PersonenUitKszStub(command: command),
                cancellationToken: CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _newAggregateSessionMock
            .SaveInvocations.Single()
            .Vereniging.UncommittedEvents.OfType<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .Should()
            .HaveCount(expected: 1);
    }
}
