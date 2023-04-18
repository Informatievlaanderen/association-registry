namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using AutoFixture;
using Framework;
using Framework.MagdaMocks;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Required_Fields
{
    private const string Naam = "naam1";

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Required_Fields()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAll();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var command = new RegistreerVerenigingCommand(
            VerenigingsNaam.Create(Naam),
            null,
            null,
            Startdatum.Leeg,
            KboNummer.Leeg,
            Array.Empty<Contactgegeven>(),
            Array.Empty<Locatie>(),
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, new MagdaFacadeEchoMock(), new NoDuplicateVerenigingDetectionService(), clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerVerenigingCommand>(command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingWerdGeregistreerd(
                VCode: _vCodeService.GetLast(),
                Naam: Naam,
                KorteNaam: string.Empty,
                KorteBeschrijving: string.Empty,
                Startdatum: null,
                KboNummer: string.Empty,
                Contactgegevens: Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Locaties: Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Vertegenwoordigers: Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                HoofdactiviteitenVerenigingsloket: Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()));
    }
}
