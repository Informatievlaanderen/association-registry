namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.CommandHandling;

using Acties.RegistreerAfdeling;
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
public class With_A_Startdatum_In_The_Future
{
    private readonly CommandEnvelope<RegistreerAfdelingCommand> _commandEnvelope;
    private readonly RegistreerAfdelingCommandHandler _commandHandler;

    public With_A_Startdatum_In_The_Future()
    {
        var fixture = new Fixture().CustomizeAll();
        var repositoryMock = new VerenigingRepositoryMock();
        var today = fixture.Create<DateOnly>();

        var command = fixture.Create<RegistreerAfdelingCommand>() with
        {
            Startdatum = Startdatum.Create(today.AddDays(value: 1)),
        };

        var commandMetadata = fixture.Create<CommandMetadata>();
        _commandHandler = new RegistreerAfdelingCommandHandler(
            repositoryMock,
            new InMemorySequentialVCodeService(),
            new NoDuplicateVerenigingDetectionService(),
            new ClockStub(today));

        _commandEnvelope = new CommandEnvelope<RegistreerAfdelingCommand>(command, commandMetadata);
    }

    [Fact]
    public async Task Then_it_throws_an_StartdatumIsInFutureException()
    {
        var method = () => _commandHandler.Handle(_commandEnvelope, CancellationToken.None);
        await method.Should().ThrowAsync<StardatumIsInFuture>();
    }
}
