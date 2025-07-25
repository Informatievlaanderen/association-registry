﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.Clients;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DuplicateVerenigingDetection;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging.Geotags;
using Wolverine.Marten;
using Xunit;

public class With_A_Startdatum_On_Today
{
    private const string Naam = "naam1";
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly GeotagsCollection _geotagsCollection;

    public With_A_Startdatum_On_Today()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        var vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var command = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with { Naam = VerenigingsNaam.Create(Naam) };
        var commandMetadata = fixture.Create<CommandMetadata>();

        var (geotagService, geotagsCollection) = Faktory.New(fixture).GeotagsService.ReturnsRandomGeotags();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            _verenigingRepositoryMock,
            vCodeService,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            geotagService.Object,
            NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        commandHandler
           .Handle(
                new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata),
                VerrijkteAdressenUitGrar.Empty,
                PotentialDuplicatesFound.None,
                CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.SaveInvocations
                                 .Single().Vereniging.UncommittedEvents
                                 .OfType< VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                 .Should().HaveCount(expected: 1);
    }
}
