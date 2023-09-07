﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using FluentAssertions;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_On_Today
{
    private const string Naam = "naam1";
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_A_Startdatum_On_Today()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        var vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>() with { Naam = VerenigingsNaam.Create(Naam) };
        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(
            _verenigingRepositoryMock,
            vCodeService,
            new NoDuplicateVerenigingDetectionService(),
            new ClockStub(command.Startdatum.Value));

        commandHandler
           .Handle(new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.SaveInvocations
                                 .Single().Vereniging.UncommittedEvents
                                 .OfType<FeitelijkeVerenigingWerdGeregistreerd>()
                                 .Should().HaveCount(expected: 1);
    }
}
