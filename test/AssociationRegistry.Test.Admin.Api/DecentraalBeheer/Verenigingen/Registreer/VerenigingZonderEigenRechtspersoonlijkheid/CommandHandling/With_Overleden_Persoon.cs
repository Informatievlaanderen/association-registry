namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Middleware;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
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
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Resources;
using ResultNet;
using Wolverine.Marten;
using Xunit;

public class With_Overleden_Persoon
{


    [Fact]
    public async ValueTask Then_The_Result_Is_A_Failure()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var faktory = new Faktory(fixture);

        var command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>();

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            new VerenigingRepositoryMock(),
            new InMemorySequentialVCodeService(),
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            faktory.GeotagsService.ReturnsEmptyGeotags().Object,
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        var exception = await Assert.ThrowsAsync<VerenigingKanNietGeregistreerdWordenMetOverledenVertegenwoordigers>(() =>
            commandHandler.Handle(
                                 new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                                     command, commandMetadata),
                                 VerrijkteAdressenUitGrar.Empty,
                                 PotentialDuplicatesFound.None,
                                 new PersonenUitKszStub(command, anyOverleden: true),
                                 CancellationToken.None));

        exception.Message.Should().BeEquivalentTo(ExceptionMessages.VerenigingKanNietGeregistreerdWordenMetOverledenVertegenwoordigers);
    }
}
