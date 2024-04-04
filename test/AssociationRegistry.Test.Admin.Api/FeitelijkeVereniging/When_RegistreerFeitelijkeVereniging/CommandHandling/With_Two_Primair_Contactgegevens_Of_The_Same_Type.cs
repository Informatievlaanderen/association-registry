﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using FluentAssertions;
using Framework;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_The_Same_Type
{
    private readonly CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> _commandEnvelope;
    private readonly RegistreerFeitelijkeVerenigingCommandHandler _commandHandler;

    public With_Two_Primair_Contactgegevens_Of_The_Same_Type()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var repositoryMock = new VerenigingRepositoryMock();

        var contactgegeven =
            Contactgegeven.CreateFromInitiator(Contactgegeventype.Email, waarde: "test@example.org", fixture.Create<string>(),
                                               isPrimair: true);

        var contactgegeven2 =
            Contactgegeven.CreateFromInitiator(Contactgegeventype.Email, waarde: "test2@example.org", fixture.Create<string>(),
                                               isPrimair: true);

        var command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>() with
        {
            Contactgegevens = new[]
            {
                contactgegeven,
                contactgegeven2,
            },
        };

        var commandMetadata = fixture.Create<CommandMetadata>();

        _commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new NoDuplicateVerenigingDetectionService(),
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>(),
            new ClockStub(command.Startdatum.Value),
            NullLogger<RegistreerFeitelijkeVerenigingCommandHandler>.Instance);

        _commandEnvelope = new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_The_Result_Contains_The_Potential_Duplicates()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<MeerderePrimaireContactgegevensZijnNietToegestaan>();
    }
}
