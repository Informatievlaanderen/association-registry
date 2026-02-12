namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.CommandHandling.Bankrekeningnummers;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid.DuplicateVerenigingDetection;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bankrekeningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Stubs.VCodeServices;
using Common.StubsMocksFakes;
using Common.StubsMocksFakes.Clocks;
using Common.StubsMocksFakes.Faktories;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Events.Factories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Resources;
using Wolverine.Marten;
using Xunit;

public class WIth_Duplicate_Ibans
{
    private const string Naam = "naam1";
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly NewAggregateSessionMock _newAggregateSessionMock;
    private RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand _command;
    private RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler _commandHandler;
    private Fixture _fixture;

    public WIth_Duplicate_Ibans()
    {
        _newAggregateSessionMock = new NewAggregateSessionMock();
        _vCodeService = new InMemorySequentialVCodeService();

        _fixture = new Fixture().CustomizeAdminApi();
        var iban = _fixture.Create<IbanNummer>();
        var geotagService = Faktory.New(fixture: _fixture).GeotagsService.ReturnsEmptyGeotags();

        var today = _fixture.Create<DateOnly>();

        var clock = new ClockStub(now: today);

        _command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            OriginalRequest: _fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>(),
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
            Werkingsgebieden: [],
            Bankrekeningnummers:
            [
                _fixture.Create<ToeTevoegenBankrekeningnummer>() with
                {
                    Iban = iban,
                },
                _fixture.Create<ToeTevoegenBankrekeningnummer>() with
                {
                    Iban = iban,
                },
            ]
        );

        _commandHandler = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
            newAggregateSession: _newAggregateSessionMock,
            vCodeService: _vCodeService,
            outbox: Mock.Of<IMartenOutbox>(),
            session: Mock.Of<IDocumentSession>(),
            clock: clock,
            geotagsService: geotagService.Object,
            logger: NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance
        );
    }

    [Fact]
    public async Task Then_Throws_Iban_()
    {
        var exception = await Assert.ThrowsAsync<IbanMoetUniekZijn>(testCode: async () =>
            await _commandHandler.Handle(
                message: new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(
                    Command: _command,
                    Metadata: _fixture.Create<CommandMetadata>()
                ),
                verrijkteAdressenUitGrar: VerrijkteAdressenUitGrar.Empty,
                potentialDuplicates: PotentialDuplicatesFound.None,
                personenUitKsz: new PersonenUitKszStub(command: _command),
                cancellationToken: CancellationToken.None
            )
        );

        exception.Message.Should().Be(expected: ExceptionMessages.IbanMoetUniekZijn);
    }
}
