namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.CommandHandling;

using Acties.RegistreerFeitelijkeVereniging;
using Events;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using Vereniging;
using AutoFixture;
using Xunit;
using Xunit.Categories;

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

        var fixture = new Fixture().CustomizeAdminApi();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var command = new RegistreerFeitelijkeVerenigingCommand(
            VerenigingsNaam.Create(Naam),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum.Leeg,
            false,
            Array.Empty<Contactgegeven>(),
            Array.Empty<Locatie>(),
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandMetadata = fixture.Create<CommandMetadata>();
        var commandHandler = new RegistreerFeitelijkeVerenigingCommandHandler(_verenigingRepositoryMock, _vCodeService, new NoDuplicateVerenigingDetectionService(), clock);

        commandHandler
            .Handle(new CommandEnvelope<RegistreerFeitelijkeVerenigingCommand>(command, commandMetadata), CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new FeitelijkeVerenigingWerdGeregistreerd(
                _vCodeService.GetLast(),
                Naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()));
    }
}
