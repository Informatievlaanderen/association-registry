namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events.Factories;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine.Marten;
using Xunit;

public class With_WerkingsgebiedenWerdenNietBepaald
{
    private const string Naam = "naam1";
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly NewAggregateSessionMock _newAggregateSessionMock;

    public With_WerkingsgebiedenWerdenNietBepaald()
    {
        _newAggregateSessionMock = new NewAggregateSessionMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi().WithoutWerkingsgebieden();
        var geotagService = Faktory.New(fixture: fixture).GeotagsService.ReturnsEmptyGeotags();

        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(now: today);

        var command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            OriginalRequest: fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>(),
            Naam: VerenigingsNaam.Create(naam: Naam),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep: Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Contactgegevens: [],
            Locaties: [],
            Vertegenwoordigers: [],
            HoofdactiviteitenVerenigingsloket: [],
            Werkingsgebieden: Werkingsgebieden.NietBepaald,
            Bankrekeningnummers: []
        );

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: _newAggregateSessionMock,
            vCodeService: _vCodeService,
            outbox: Mock.Of<IMartenOutbox>(),
            session: Mock.Of<IDocumentSession>(),
            clock: clock,
            geotagsService: geotagService.Object,
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );

        commandHandler
            .Handle(
                message: new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                    Command: command,
                    Metadata: commandMetadata
                ),
                verrijkteAdressenUitGrar: VerrijkteAdressenUitGrar.Empty,
                potentialDuplicates: PotentialDuplicatesFound.None,
                personenUitKsz: new PersonenUitKszStub(command: command),
                cancellationToken: CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        var vCode = _vCodeService.GetLast();

        _newAggregateSessionMock.ShouldHaveSavedExact(
            events:
            [
                new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                    VCode: vCode,
                    Naam: Naam,
                    KorteNaam: string.Empty,
                    KorteBeschrijving: string.Empty,
                    Startdatum: null,
                    Doelgroep: EventFactory.Doelgroep(doelgroep: Doelgroep.Null),
                    IsUitgeschrevenUitPubliekeDatastroom: false,
                    Contactgegevens: [],
                    Locaties: [],
                    Vertegenwoordigers: [],
                    HoofdactiviteitenVerenigingsloket: [],
                    Bankrekeningnummers: [],
                    DuplicatieInfo: Registratiedata.DuplicatieInfo.GeenDuplicaten
                ),
                new GeotagsWerdenBepaald(VCode: vCode, Geotags: []),
            ]
        );
    }
}
