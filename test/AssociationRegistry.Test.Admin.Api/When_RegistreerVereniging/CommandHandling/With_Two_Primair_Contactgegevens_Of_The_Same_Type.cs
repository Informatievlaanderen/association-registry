namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using AssociationRegistry.Framework;
using Magda;
using Primitives;
using Fakes;
using Framework;
using Vereniging.DuplicateDetection;
using Vereniging.RegistreerVereniging;
using AutoFixture;
using ContactGegevens;
using ContactGegevens.Exceptions;
using FluentAssertions;
using Moq;
using Startdatums;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_The_Same_Type
{
    private readonly CommandEnvelope<RegistreerVerenigingCommand> _commandEnvelope;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;

    public With_Two_Primair_Contactgegevens_Of_The_Same_Type()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var contactgegeven = new RegistreerVerenigingCommand.Contactgegeven(ContactgegevenType.Email, "test@example.org", fixture.Create<string>(), true);
        var command = new RegistreerVerenigingCommand(
            fixture.Create<string>(),
            null,
            null,
            Startdatum.Leeg,
            null,
            new[]
            {
                contactgegeven,
                contactgegeven with { Waarde = $"2{contactgegeven.Waarde}" },
            },
            Array.Empty<RegistreerVerenigingCommand.Locatie>(),
            Array.Empty<RegistreerVerenigingCommand.Vertegenwoordiger>(),
            Array.Empty<string>(),
            true);

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            Mock.Of<IMagdaFacade>(),
            Mock.Of<IDuplicateDetectionService>(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_The_Result_Contains_The_Potential_Duplicates()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<MultiplePrimaryContactgegevens>();
    }
}
