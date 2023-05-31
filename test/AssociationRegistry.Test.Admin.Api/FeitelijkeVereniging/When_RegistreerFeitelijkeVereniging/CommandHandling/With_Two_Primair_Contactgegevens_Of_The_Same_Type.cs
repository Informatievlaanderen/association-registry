namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging;
using Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Two_Primair_Contactgegevens_Of_The_Same_Type
{
    private readonly CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> _commandEnvelope;
    private readonly RegistreerFeitelijkeVerenigingCommandHandler _commandHandler;

    public With_Two_Primair_Contactgegevens_Of_The_Same_Type()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();

        var contactgegeven = Contactgegeven.Create(ContactgegevenType.Email, "test@example.org", fixture.Create<string>(), isPrimair: true);
        var contactgegeven2 = Contactgegeven.Create(ContactgegevenType.Email, "test2@example.org", fixture.Create<string>(), isPrimair: true);
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
            new ClockStub(command.Startdatum.Datum!.Value));

        _commandEnvelope = new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_The_Result_Contains_The_Potential_Duplicates()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<MultiplePrimaryContactgegevens>();
    }
}
