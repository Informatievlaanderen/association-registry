namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using AutoFixture;
using FluentAssertions;
using Framework.MagdaMocks;
using Vereniging;
using Vereniging.Exceptions;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Startdatum_In_The_Future
{
    private readonly CommandEnvelope<RegistreerVerenigingCommand> _commandEnvelope;
    private readonly RegistreerVerenigingCommandHandler _commandHandler;

    public With_A_Startdatum_In_The_Future()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var command = fixture.Create<RegistreerVerenigingCommand>() with
        {
            Startdatum = Startdatum.Create(today.AddDays(1)),
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new RegistreerVerenigingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new MagdaFacadeEchoMock(),
            new NoDuplicateVerenigingDetectionService(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_it_throws_an_invalidStartdatumFutureException()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<StardatumIsInFuture>();
    }
}
