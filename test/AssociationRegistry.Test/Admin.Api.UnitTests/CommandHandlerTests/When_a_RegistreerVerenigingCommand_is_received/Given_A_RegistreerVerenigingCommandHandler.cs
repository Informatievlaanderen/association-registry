namespace AssociationRegistry.Test.Admin.Api.UnitTests.CommandHandlerTests.When_a_RegistreerVerenigingCommand_is_received;

using AssociationRegistry.Framework;
using AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    public record Invocation(Vereniging Vereniging);

    public readonly List<Invocation> Invocations = new();

    public async Task Save(Vereniging vereniging, CommandMetadata metadata)    {
        Invocations.Add(new Invocation(vereniging));
        await Task.CompletedTask;
    }
}

public class Given_A_RegistreerVerenigingCommandHandler
{
    private readonly Fixture _fixture;

    public Given_A_RegistreerVerenigingCommandHandler()
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
            new RegistreerVerenigingCommand("naam1", null, null, null, null),
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
        theEvent.Status.Should().Be("Actief");
        theEvent.DatumLaatsteAanpassing.Should().Be(clock.Today);
    }

    [Fact]
    public async Task Given_All_fields_Then_a_new_vereniging_is_saved_in_the_repository()
    {
        var vNummerService = new InMemorySequentialVCodeService();
        var verenigingsRepository = new VerenigingRepositoryMock();
        var today = _fixture.Create<DateTime>();
        var clock = new ClockStub(today);
        var startdatumInThePast = today.AddDays(-3);

        var handler = new RegistreerVerenigingCommandHandler(verenigingsRepository, vNummerService, clock);
        var registreerVerenigingCommand = new CommandEnvelope<RegistreerVerenigingCommand>(
            new RegistreerVerenigingCommand("naam1", "korte naam", "korte beschrijving", DateOnly.FromDateTime(startdatumInThePast), "0123456749"),
            _fixture.Create<CommandMetadata>());

        await handler.Handle(registreerVerenigingCommand, CancellationToken.None);

        var invocation = verenigingsRepository.Invocations.Single();
        invocation.Vereniging.VCode.Should().Be(vNummerService.GetLast());
        var theEvent = (VerenigingWerdGeregistreerd)invocation.Vereniging.Events.Single();

        theEvent.VCode.Should().Be(vNummerService.GetLast());
        theEvent.Naam.Should().Be("naam1");
        theEvent.KorteNaam.Should().Be("korte naam");
        theEvent.KorteBeschrijving.Should().Be("korte beschrijving");
        theEvent.Startdatum.Should().Be(DateOnly.FromDateTime(startdatumInThePast));
        theEvent.KboNummer.Should().Be("0123456749");
        theEvent.Status.Should().Be("Actief");
        theEvent.DatumLaatsteAanpassing.Should().Be(clock.Today);
    }
}
