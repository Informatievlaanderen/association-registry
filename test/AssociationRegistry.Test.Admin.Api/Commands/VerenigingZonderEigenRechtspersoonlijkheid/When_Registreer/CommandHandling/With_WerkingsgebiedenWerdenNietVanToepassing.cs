namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingZonderEigenRechtspersoonlijkheid.When_Registreer.CommandHandling;

using DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using EventFactories;
using Events;
using AssociationRegistry.Framework;
using Grar.Clients;
using Framework.Fakes;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using Vereniging;
using AutoFixture;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine.Marten;
using Xunit;

public class With_NietVanToepassing_Werkingsgebieden
{
    private const string Naam = "naam1";
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;

    public With_NietVanToepassing_Werkingsgebieden()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();
        var today = fixture.Create<DateOnly>();

        var clock = new ClockStub(today);

        var command = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand(
            VerenigingsNaam.Create(Naam),
            KorteNaam: null,
            KorteBeschrijving: null,
            Startdatum: null,
            Doelgroep.Null,
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Array.Empty<Contactgegeven>(),
            Array.Empty<Locatie>(),
            Array.Empty<Vertegenwoordiger>(),
            Array.Empty<HoofdactiviteitVerenigingsloket>(),
            Werkingsgebieden.NietVanToepassing);

        var commandMetadata = fixture.Create<CommandMetadata>();

        var commandHandler =
            new RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(_verenigingRepositoryMock,
                                                             _vCodeService,
                                                             new NoDuplicateVerenigingDetectionService(),
                                                             Mock.Of<IMartenOutbox>(),
                                                             Mock.Of<IDocumentSession>(),
                                                             clock,
                                                             Mock.Of<IGrarClient>(),
                                                             NullLogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler>.Instance);

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>(command, commandMetadata), CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_event()
    {
        var vCode = _vCodeService.GetLast();

        _verenigingRepositoryMock.ShouldHaveSaved(
            new  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                vCode,
                Naam,
                string.Empty,
                string.Empty,
                Startdatum: null,
                EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: false,
                Array.Empty<Registratiedata.Contactgegeven>(),
                Array.Empty<Registratiedata.Locatie>(),
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()
            ),
            new WerkingsgebiedenWerdenNietVanToepassing(vCode));
    }
}
