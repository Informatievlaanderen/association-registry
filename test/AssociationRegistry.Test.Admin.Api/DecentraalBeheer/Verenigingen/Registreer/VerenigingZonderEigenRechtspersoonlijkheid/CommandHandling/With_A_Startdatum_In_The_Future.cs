namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
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
using Wolverine.Marten;
using Xunit;

public class With_A_Startdatum_In_The_Future
{
    private readonly CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> _commandEnvelope;
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler _commandHandler;
    private RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;

    public With_A_Startdatum_In_The_Future()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var repositoryMock = new NewAggregateSessionMock();
        var today = fixture.Create<DateOnly>();

        _command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Startdatum = Datum.Create(startdatum: today.AddDays(value: 1)),
        };

        var commandMetadata = fixture.Create<CommandMetadata>();

        _commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: repositoryMock,
            vCodeService: new InMemorySequentialVCodeService(),
            outbox: Mock.Of<IMartenOutbox>(),
            session: Mock.Of<IDocumentSession>(),
            clock: new ClockStub(now: today),
            geotagsService: Mock.Of<IGeotagsService>(),
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
            Command: _command,
            Metadata: commandMetadata
        );
    }

    [Fact]
    public async ValueTask Then_it_throws_an_StartdatumIsInFutureException()
    {
        var method = () =>
            _commandHandler.Handle(
                message: _commandEnvelope,
                verrijkteAdressenUitGrar: VerrijkteAdressenUitGrar.Empty,
                potentialDuplicates: PotentialDuplicatesFound.None,
                personenUitKsz: new PersonenUitKszStub(command: _command),
                cancellationToken: CancellationToken.None
            );
        await method.Should().ThrowAsync<StartdatumMagNietInToekomstZijn>();
    }
}
