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
            Naam = VerenigingsNaam.Create(Naam),
        };
        var commandMetadata = fixture.Create<CommandMetadata>();

        var (geotagService, geotagsCollection) = Faktory.New(fixture).GeotagsService.ReturnsRandomGeotags();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            _newAggregateSessionMock,
            vCodeService,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            geotagService.Object,
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );

        commandHandler
            .Handle(
                new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                    command,
                    commandMetadata
                ),
                VerrijkteAdressenUitGrar.Empty,
                PotentialDuplicatesFound.None,
                new PersonenUitKszStub(command),
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
