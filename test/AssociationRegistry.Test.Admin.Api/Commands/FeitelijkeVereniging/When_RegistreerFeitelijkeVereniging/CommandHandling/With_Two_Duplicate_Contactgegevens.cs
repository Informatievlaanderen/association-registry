﻿namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Registratie.RegistreerFeitelijkeVereniging;
using FluentAssertions;
using Framework.Fakes;
using Grar;
using Grar.Clients;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Duplicate_Contactgegevens
{
    private readonly CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> _commandEnvelope;
    private readonly RegistreerFeitelijkeVerenigingCommandHandler _commandHandler;

    public With_Two_Duplicate_Contactgegevens()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var repositoryMock = new VerenigingRepositoryMock();

        var contactgegeven =
            Contactgegeven.CreateFromInitiator(Contactgegeventype.Email, waarde: "test@example.org", fixture.Create<string>(),
                                               isPrimair: true);

        var command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>() with
        {
            Contactgegevens = new[] { contactgegeven, contactgegeven },
            SkipDuplicateDetection = true,
        };

        var commandMetadata = fixture.Create<CommandMetadata>();

        _commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new NoDuplicateVerenigingDetectionService(),
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            Mock.Of<IGrarClient>(),
            NullLogger<RegistreerFeitelijkeVerenigingCommandHandler>.Instance);

        _commandEnvelope = new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_The_Result_Contains_The_Potential_Duplicates()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<ContactgegevenIsDuplicaat>();
    }
}
