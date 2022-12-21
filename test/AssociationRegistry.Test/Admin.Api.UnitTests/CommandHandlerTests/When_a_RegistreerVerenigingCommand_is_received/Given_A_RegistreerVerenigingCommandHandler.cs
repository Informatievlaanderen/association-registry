namespace AssociationRegistry.Test.Admin.Api.UnitTests.CommandHandlerTests.When_a_RegistreerVerenigingCommand_is_received;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Framework;
using AutoFixture;
using FluentAssertions;
using Vereniging;
using Xunit;

public class VerenigingRepositoryMock : IVerenigingsRepository
{
    public record Invocation(Vereniging Vereniging);

    public readonly List<Invocation> Invocations = new();

    public async Task Save(Vereniging vereniging, CommandMetadata metadata)
    {
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
            new RegistreerVerenigingCommand(
                "naam1",
                "korte naam",
                "korte beschrijving",
                DateOnly.FromDateTime(startdatumInThePast),
                "0123456749",
                new List<RegistreerVerenigingCommand.ContactInfo>
                {
                    new(
                        "Algemeen",
                        "info@dummy.com",
                        "1234567890",
                        "www.test-website.be",
                        "@test"),
                },
                new RegistreerVerenigingCommand.Locatie[]
                {
                    new("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, LocatieTypes.Activiteiten),
                }),
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
        theEvent.DatumLaatsteAanpassing.Should().Be(clock.Today);
        theEvent.Contacten![0].Contactnaam.Should().Be("Algemeen");
        theEvent.Contacten[0].Email.Should().Be("info@dummy.com");
        theEvent.Contacten[0].Telefoon.Should().Be("1234567890");
        theEvent.Contacten[0].Website.Should().Be("www.test-website.be");
        theEvent.Contacten[0].SocialMedia.Should().Be("@test");
        theEvent.Locaties.Should().HaveCount(1);
        theEvent.Locaties[0].HoofdLocatie.Should().BeTrue();
        theEvent.Locaties[0].Huisnummer.Should().Be("1");
        theEvent.Locaties[0].LocatieType.Should().Be(LocatieTypes.Activiteiten);
        theEvent.Locaties[0].Naam.Should().Be("Kerker");
        theEvent.Locaties[0].Busnummer.Should().Be("-1");
        theEvent.Locaties[0].Gemeente.Should().Be("penoze");
        theEvent.Locaties[0].Land.Should().Be("Nederland");
        theEvent.Locaties[0].Postcode.Should().Be("666");
        theEvent.Locaties[0].Straatnaam.Should().Be("kerkstraat");
    }
}
