namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.CommandHandling;

using Acties.RegistreerVereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Framework.MagdaMocks;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Vertegenwoordiger = Vereniging.Vertegenwoordiger;

[UnitTest]
public class With_Required_Fields
{
    private const string Naam = "naam1";
    private readonly InMemorySequentialVCodeService _vCodeService;

    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_Required_Fields()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAll();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var command = new RegistreerVerenigingCommand(
            VerenigingsNaam.Create(Naam),
            KorteNaam: null,
            KorteBeschrijving: null,
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
                _vCodeService.GetLast(),
                Naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                string.Empty,
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                Array.Empty<VerenigingWerdGeregistreerd.Locatie>(),
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()));
    }
}
