namespace AssociationRegistry.Test.Admin.Api.CommandHandler.When_handling_a_RegistreerVerenigingCommand;

using AutoFixture;
using FluentAssertions;
using global::AssociationRegistry.Framework;
using Vereniging;
using Xunit;

public class Given_A_RegistreerVerenigingCommand_With_Minimal_Data
{
    private readonly Fixture _fixture;

    public Given_A_RegistreerVerenigingCommand_With_Minimal_Data()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Then_a_new_vereniging_is_saved_in_the_repository()
    {
        var vNummerService = new InMemorySequentialVCodeService();
        var verenigingsRepository = new VerenigingRepositoryMock();
        var clock = new ClockStub(_fixture.Create<DateTime>());

        var handler = new RegistreerVerenigingCommandHandler(verenigingsRepository, vNummerService, clock);
        var registreerVerenigingCommand = new CommandEnvelope<RegistreerVerenigingCommand>(
            RegistreerVerenigingCommand("naam1"),
            _fixture.Create<CommandMetadata>());

        await handler.Handle(registreerVerenigingCommand, CancellationToken.None);

        var invocation = verenigingsRepository.Invocations.Single();
        invocation.Vereniging.VCode.Should().Be(vNummerService.GetLast());
        var theEvent = (VerenigingWerdGeregistreerd)invocation.Vereniging.Events.Single();

        theEvent.VCode.Should().Be(vNummerService.GetLast());
        theEvent.Naam.Should().Be("naam1");
        theEvent.KorteNaam.Should().BeNull();
        theEvent.KorteBeschrijving.Should().BeNull();
        theEvent.Startdatum.Should().BeNull();
        theEvent.KboNummer.Should().BeNull();
        theEvent.DatumLaatsteAanpassing.Should().Be(clock.Today);
    }

    private static RegistreerVerenigingCommand RegistreerVerenigingCommand(string naam)
        => new(naam, null, null, null, null, Array.Empty<RegistreerVerenigingCommand.ContactInfo>(), Array.Empty<RegistreerVerenigingCommand.Locatie>());
}
