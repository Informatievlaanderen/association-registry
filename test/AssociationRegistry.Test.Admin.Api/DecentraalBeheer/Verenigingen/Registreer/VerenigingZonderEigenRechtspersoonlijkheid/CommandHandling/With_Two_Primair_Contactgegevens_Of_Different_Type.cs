namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling;

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

public class With_Two_Primair_Contactgegevens_Of_Different_Type : IAsyncLifetime
{
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;
    private readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler _commandHandler;
    private readonly IFixture _fixture;
    private readonly NewAggregateSessionMock _repositoryMock;
    private readonly InMemorySequentialVCodeService _vCodeService;

    public With_Two_Primair_Contactgegevens_Of_Different_Type()
    {
        _fixture = new Fixture().CustomizeAdminApi().WithoutWerkingsgebieden();

        var geotagService = Faktory.New(fixture: _fixture).GeotagsService.ReturnsEmptyGeotags();

        _repositoryMock = new NewAggregateSessionMock();

        _command = _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>() with
        {
            Contactgegevens =
            [
                Contactgegeven.CreateFromInitiator(
                    type: Contactgegeventype.Email,
                    waarde: "test@example.org",
                    beschrijving: _fixture.Create<string>(),
                    isPrimair: true
                ),
                Contactgegeven.CreateFromInitiator(
                    type: Contactgegeventype.Website,
                    waarde: "http://example.org",
                    beschrijving: _fixture.Create<string>(),
                    isPrimair: true
                ),
            ],
            Bankrekeningnummers = [],
        };

        _vCodeService = new InMemorySequentialVCodeService();

        _commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: _repositoryMock,
            vCodeService: _vCodeService,
            outbox: Mock.Of<IMartenOutbox>(),
            session: Mock.Of<IDocumentSession>(),
            clock: new ClockStub(now: _command.Startdatum.Value),
            geotagsService: geotagService.Object,
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );
    }

    public async ValueTask InitializeAsync()
    {
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await _commandHandler.Handle(
            message: new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                Command: _command,
                Metadata: commandMetadata
            ),
            verrijkteAdressenUitGrar: VerrijkteAdressenUitGrar.Empty,
            potentialDuplicates: PotentialDuplicatesFound.None,
            personenUitKsz: new PersonenUitKszStub(command: _command),
            cancellationToken: CancellationToken.None
        );
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public void Then_it_saves_the_event()
    {
        var vCode = _vCodeService.GetLast();

        _repositoryMock.ShouldHaveSavedExact(
            events:
            [
                new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                    VCode: vCode,
                    Naam: _command.Naam,
                    KorteNaam: _command.KorteNaam ?? string.Empty,
                    KorteBeschrijving: _command.KorteBeschrijving ?? string.Empty,
                    Startdatum: _command.Startdatum,
                    Doelgroep: EventFactory.Doelgroep(doelgroep: _command.Doelgroep),
                    IsUitgeschrevenUitPubliekeDatastroom: _command.IsUitgeschrevenUitPubliekeDatastroom,
                    Contactgegevens:
                    [
                        new Registratiedata.Contactgegeven(
                            ContactgegevenId: 1,
                            Contactgegeventype: Contactgegeventype.Email,
                            Waarde: _command.Contactgegevens[0].Waarde,
                            Beschrijving: _command.Contactgegevens[0].Beschrijving,
                            IsPrimair: _command.Contactgegevens[0].IsPrimair
                        ),
                        new Registratiedata.Contactgegeven(
                            ContactgegevenId: 2,
                            Contactgegeventype: Contactgegeventype.Website,
                            Waarde: _command.Contactgegevens[1].Waarde,
                            Beschrijving: _command.Contactgegevens[1].Beschrijving,
                            IsPrimair: _command.Contactgegevens[1].IsPrimair
                        ),
                    ],
                    Locaties: _command
                        .Locaties.Select(
                            selector: (l, index) => EventFactory.Locatie(locatie: l) with { LocatieId = index + 1 }
                        )
                        .ToArray(),
                    Vertegenwoordigers: _command
                        .Vertegenwoordigers.Select(
                            selector: (v, index) =>
                                EventFactory.Vertegenwoordiger(vertegenwoordiger: v) with
                                {
                                    VertegenwoordigerId = index + 1,
                                }
                        )
                        .ToArray(),
                    HoofdactiviteitenVerenigingsloket: _command
                        .HoofdactiviteitenVerenigingsloket.Select(
                            selector: h => new Registratiedata.HoofdactiviteitVerenigingsloket(
                                Code: h.Code,
                                Naam: h.Naam
                            )
                        )
                        .ToArray(),
                    Bankrekeningnummers: [],
                    DuplicatieInfo: Registratiedata.DuplicatieInfo.GeenDuplicaten
                ),
                new GeotagsWerdenBepaald(VCode: vCode, Geotags: []),
            ]
        );
    }
}
