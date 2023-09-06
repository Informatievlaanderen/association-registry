﻿namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.CommandHandling;

using Acties.RegistreerAfdeling;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Fakes;
using Framework;
using Vereniging;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Existing_Moeder
{
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly KboNummer _kboNummerMoedervereniging;
    private readonly VerenigingsNaam _verenigingsNaam;
    private readonly VerenigingsRepository.VCodeAndNaam _moederVCodeAndNaam;

    public With_An_Existing_Moeder()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        _kboNummerMoedervereniging = fixture.Create<KboNummer>();
        _verenigingsNaam = fixture.Create<VerenigingsNaam>();

        _moederVCodeAndNaam = new VerenigingsRepository.VCodeAndNaam(fixture.Create<VCode>(), fixture.Create<VerenigingsNaam>());
        _verenigingRepositoryMock = new VerenigingRepositoryMock(moederVCodeAndNaam: _moederVCodeAndNaam);
        _vCodeService = new InMemorySequentialVCodeService();

        var today = fixture.Create<DateOnly>();
        var clock = new ClockStub(today);

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
                    _moederVCodeAndNaam.VCode!,
                    _moederVCodeAndNaam.VerenigingsNaam),
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
