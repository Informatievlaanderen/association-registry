namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Fakes;
using FluentAssertions;
using Framework;
using Framework.MagdaMocks;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_In_The_Future
{
    private readonly CommandEnvelope<RegistreerFeitelijkeVerenigingCommand> _commandEnvelope;
    private readonly RegistreerFeitelijkeVerenigingCommandHandler _commandHandler;

    public With_A_Startdatum_In_The_Future()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var command = fixture.Create<RegistreerFeitelijkeVerenigingCommand>() with
        {
            Startdatum = Startdatum.Create(today.AddDays(value: 1)),
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new MagdaFacadeEchoMock(),
            new NoDuplicateVerenigingDetectionService(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_it_throws_an_StartdatumIsInFutureException()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<StardatumIsInFuture>();
    }
}
