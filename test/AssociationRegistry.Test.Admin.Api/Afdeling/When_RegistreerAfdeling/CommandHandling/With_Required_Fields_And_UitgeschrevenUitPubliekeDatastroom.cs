namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.CommandHandling;

using Acties.RegistreerAfdeling;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Required_Fields_And_UitgeschrevenUitPubliekeDatastroom
{
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly KboNummer _kboNummerMoedervereniging;
    private readonly VerenigingsNaam _verenigingsNaam;

    public With_Required_Fields_And_UitgeschrevenUitPubliekeDatastroom()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        _kboNummerMoedervereniging = fixture.Create<KboNummer>();
        _verenigingsNaam = fixture.Create<VerenigingsNaam>();

        var command = new RegistreerAfdelingCommand(
            _verenigingsNaam,
            _kboNummerMoedervereniging,
            KorteNaam: null,
            KorteBeschrijving: null,
            StartDatum: null,
            Doelgroep.Null,
            Array.Empty<Contactgegeven>(),
            Array.Empty<Locatie>(),
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>());

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerAfdelingCommandHandler(
            _verenigingRepositoryMock,
            _vCodeService,
            new NoDuplicateVerenigingDetectionService(),
            clock);

        commandHandler
           .Handle(new CommandEnvelope<RegistreerAfdelingCommand>(command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new AfdelingWerdGeregistreerd(
                _vCodeService.GetLast(),
                _verenigingsNaam,
                new AfdelingWerdGeregistreerd.MoederverenigingsData(
                    _kboNummerMoedervereniging,
                    string.Empty,
                    $"Moeder {_kboNummerMoedervereniging}"),
                string.Empty,
                string.Empty,
                Startdatum: null,
                Registratiedata.Doelgroep.With(Doelgroep.Null),
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()));
    }
}
