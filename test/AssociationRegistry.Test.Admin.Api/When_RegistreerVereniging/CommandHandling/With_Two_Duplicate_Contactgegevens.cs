﻿namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using Contactgegevens;
using Contactgegevens.Exceptions;
using FluentAssertions;
using Framework.MagdaMocks;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Duplicate_Contactgegevens
{
    private readonly CommandEnvelope<RegistreerVerenigingCommand> _commandEnvelope;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;

    public With_Two_Duplicate_Contactgegevens()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();

        var contactgegeven = Contactgegeven.Create(ContactgegevenType.Email, "test@example.org", fixture.Create<string>(), true);

        var command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Contactgegevens = new[] { contactgegeven, contactgegeven },
            SkipDuplicateDetection = true,
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new MagdaFacadeEchoMock(),
            new NoDuplicateDetectionService(),
            new ClockStub(command.Startdatum.Datum!.Value));

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_The_Result_Contains_The_Potential_Duplicates()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<DuplicateContactgegeven>();
    }
}
