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

    public With_A_Startdatum_In_The_Future()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Startdatum = Datum.Create(today.AddDays(value: 1)),
        };

        var commandMetadata = fixture.Create<CommandMetadata>();

        _commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(today),
            Mock.Of<IGeotagsService>(),
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata);
    }

    [Fact]
    public async ValueTask Then_it_throws_an_StartdatumIsInFutureException()
    {
        var method = () => _commandHandler.Handle(
            _commandEnvelope,
            VerrijkteAdressenUitGrar.Empty,
            PotentialDuplicatesFound.None,
            CancellationToken.None);
        await method.Should().ThrowAsync<StartdatumMagNietInToekomstZijn>();
    }
}
