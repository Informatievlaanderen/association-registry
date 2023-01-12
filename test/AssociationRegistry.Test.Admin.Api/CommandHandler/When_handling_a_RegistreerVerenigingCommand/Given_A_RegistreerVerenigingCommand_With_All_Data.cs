namespace AssociationRegistry.Test.Admin.Api.CommandHandler.When_handling_a_RegistreerVerenigingCommand;

using AutoFixture;
using FluentAssertions;
using global::AssociationRegistry.Admin.Api.Constants;
using global::AssociationRegistry.Framework;
using Vereniging;
using Xunit;

public class Given_A_RegistreerVerenigingCommand_With_All_Data
{
    private readonly Fixture _fixture;

    public Given_A_RegistreerVerenigingCommand_With_All_Data()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Then_a_new_vereniging_is_saved_in_the_repository()
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
                    new("Kerker", "kerkstraat", "1", "-1", "666", "penoze", "Nederland", true, Locatietypes.Activiteiten),
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
        theEvent.ContactInfoLijst.Should().HaveCount(1);
        theEvent.ContactInfoLijst![0].Should().BeEquivalentTo(registreerVerenigingCommand.Command.ContactInfoLijst!.First());
        theEvent.Locaties.Should().HaveCount(1);
        theEvent.Locaties![0].Should().BeEquivalentTo(registreerVerenigingCommand.Command.Locaties!.First());
    }
}
