namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using AutoFixture;
using FluentAssertions;
using Framework;
using Framework.MagdaMocks;
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

        var fixture = new Fixture().CustomizeAll();

        var command = fixture.Create<RegistreerVerenigingCommand>() with { Naam = VerenigingsNaam.Create(Naam) };
        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(
            _verenigingRepositoryMock,
            vCodeService,
            new MagdaFacadeEchoMock(),
            new NoDuplicateVerenigingDetectionService(),
            new ClockStub(command.Startdatum.Datum!.Value));

        commandHandler
            .Handle(new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.SaveInvocations
            .Single().Vereniging.UncommittedEvents
            .OfType<VerenigingWerdGeregistreerd>()
            .Should().HaveCount(1);
    }
}
